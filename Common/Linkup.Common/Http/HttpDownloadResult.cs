﻿/*
********************************************************************
*
*    曹旭升（sheng.c）
*    E-mail: cao.silhouette@msn.com
*    QQ: 279060597
*    https://github.com/iccb1013
*    http://shengxunwei.com
*
*    © Copyright 2017
*
********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.Common
{
    public class HttpDownloadResult
    {
        public bool Success
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 返回保存的文件名，仅仅文件名
        /// </summary>
        public string StoreFileName
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// 文件长度（字节）
        /// </summary>
        public long FileLength
        {
            get;
            set;
        }
    }
}
