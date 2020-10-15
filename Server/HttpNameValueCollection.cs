﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Server
{
    class HttpNameValueCollection
    {
        public class File
        {
            private string _fileName;
            public string FileName { get { return _fileName ?? (_fileName = ""); } set { _fileName = value; } }

            private string _fileData;
            public string FileData { get { return _fileData ?? (_fileName = ""); } set { _fileData = value; } }

            private string _contentType;
            public string ContentType { get { return _contentType ?? (_contentType = ""); } set { _contentType = value; } }
        }

        private NameValueCollection _get;
        private Dictionary<string, File> _files;
        private readonly HttpListenerContext _ctx;

        public NameValueCollection Get { get { return _get ?? (_get = new NameValueCollection()); } set { _get = value; } }
        public NameValueCollection Post { get { return _ctx.Request.QueryString; } }
        public Dictionary<string, File> Files { get { return _files ?? (_files = new Dictionary<string, File>()); } set { _files = value; } }

        private void PopulatePostMultiPart(string post_string)
        {
            var boundary_index = _ctx.Request.ContentType.IndexOf("boundary=") + 9;
            var boundary = _ctx.Request.ContentType.Substring(boundary_index, _ctx.Request.ContentType.Length - boundary_index);

            var upper_bound = post_string.Length - 4;

            if (post_string.Substring(2, boundary.Length) != boundary)
                throw (new InvalidDataException());

            var raw_post_strings = new List<string>();
            var current_string = new StringBuilder();

            for (var x = 4 + boundary.Length; x < upper_bound; ++x)
            {
                if (post_string.Substring(x, boundary.Length) == boundary)
                {
                    x += boundary.Length + 1;
                    raw_post_strings.Add(current_string.ToString().Remove(current_string.Length - 3, 3));
                    current_string.Clear();
                    continue;
                }

                current_string.Append(post_string[x]);

                var post_variable_string = current_string.ToString();

                var end_of_header = post_variable_string.IndexOf("\r\n\r\n");

                if (end_of_header == -1) throw (new InvalidDataException());

                var filename_index = post_variable_string.IndexOf("filename=\"", 0, end_of_header);
                var filename_starts = filename_index + 10;
                var content_type_starts = post_variable_string.IndexOf("Content-Type: ", 0, end_of_header) + 14;
                var name_starts = post_variable_string.IndexOf("name=\"") + 6;
                var data_starts = end_of_header + 4;

                if (filename_index == -1) continue;

                var filename = post_variable_string.Substring(filename_starts, post_variable_string.IndexOf("\"", filename_starts) - filename_starts);
                var content_type = post_variable_string.Substring(content_type_starts, post_variable_string.IndexOf("\r\n", content_type_starts) - content_type_starts);
                var file_data = post_variable_string.Substring(data_starts, post_variable_string.Length - data_starts);
                var name = post_variable_string.Substring(name_starts, post_variable_string.IndexOf("\"", name_starts) - name_starts);
                Files.Add(name, new File() { FileName = filename, ContentType = content_type, FileData = file_data });
                continue;

            }
        }

        private void PopulatePost()
        {
            if (_ctx.Request.HttpMethod != "POST" || _ctx.Request.ContentType == null) return;

            var post_string = new StreamReader(_ctx.Request.InputStream, _ctx.Request.ContentEncoding).ReadToEnd();

            if (_ctx.Request.ContentType.StartsWith("multipart/form-data"))
                PopulatePostMultiPart(post_string);
            else
                Get = HttpUtility.ParseQueryString(post_string);

        }

        public HttpNameValueCollection(ref HttpListenerContext ctx)
        {
            _ctx = ctx;
            PopulatePost();
        }
    }
}
