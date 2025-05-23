using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using Unity.UOS.COSXML.Common;
using Unity.UOS.COSXML.Model.Object;
using Unity.UOS.COSXML.CosException;

namespace Unity.UOS.COSXML.Model.CI
{
    /// <summary>
    /// 获取媒体信息
    /// <see href="https://cloud.tencent.com/document/product/436/55672"/>
    /// </summary>
    public sealed class GetMediaInfoRequest : ObjectRequest
    {
        public GetMediaInfoRequest(string bucket, string key)
            : base(bucket, key)
        {
            this.method = CosRequestMethod.GET;
            this.queryParameters.Add("ci-process", "videoinfo");
        }

    }
}
