using System;
using System.Collections.Generic;

namespace BaiduAutoDownloader
{
    public class OpenApiAuthException : Exception
    {
        public int Errno { get; }
        public OpenApiAuthException(int errno, string message) : base(message)
        {
            Errno = errno;
        }
    }

    public class ResourceItem
    {
        public string Title { get; set; }
        public string[] Urls { get; set; }
        public string Pwd { get; set; }
        public string ZipPassword { get; set; }
    }

    public class BaiduWebVerifyResponse
    {
        public int errno { get; set; }
        public string request_id { get; set; }
    }

    public class BaiduWebWxListResponse
    {
        public int errno { get; set; }
        public WxListData data { get; set; }

        public class WxListData
        {
            public List<WxFileMeta> list { get; set; }
        }
    }

    public class WxFileMeta
    {
        public long fs_id { get; set; }
        public string server_filename { get; set; }
        public int isdir { get; set; }
        public string path { get; set; }
        public long size { get; set; }
    }

    public class BaiduWebTransferResponse
    {
        public int errno { get; set; }
        public List<TransferInfo> info { get; set; } // When async=0
        public int taskid { get; set; } // When async=1
        public object extra { get; set; }

        public class TransferInfo
        {
            public string path { get; set; }
            public long fsid { get; set; }
        }
    }

    public class BaiduBasicFileListResponse
    {
        public int errno { get; set; }
        public List<BasicFileMeta> list { get; set; }
    }

    public class BasicFileMeta
    {
        public long fs_id { get; set; }
        public string server_filename { get; set; }
        public int isdir { get; set; }
        public string path { get; set; }
        public long size { get; set; }
    }

    public class BaiduBasicFileMetasResponse
    {
        public int errno { get; set; }
        public List<BasicFileMetaDetail> list { get; set; }
    }

    public class BasicFileMetaDetail
    {
        public long fs_id { get; set; }
        public string dlink { get; set; }
        public string server_filename { get; set; }
        public long size { get; set; }
    }
}
