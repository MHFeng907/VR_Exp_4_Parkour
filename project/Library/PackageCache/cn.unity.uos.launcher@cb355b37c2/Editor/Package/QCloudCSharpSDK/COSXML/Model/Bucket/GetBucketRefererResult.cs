using System;
using System.Collections.Generic;
using System.Text;
using Unity.UOS.COSXML.Model.Tag;

namespace Unity.UOS.COSXML.Model.Bucket
{
    /// <summary>
    /// 获取 Bucket 防盗链配置
    /// <see href="https://cloud.tencent.com/document/product/436/32493"/>
    /// </summary>
    public sealed class GetBucketRefererResult : CosDataResult<RefererConfiguration>
    {
        public RefererConfiguration refererConfiguration { 
            get{ return _data; } 
        }
    }
}
