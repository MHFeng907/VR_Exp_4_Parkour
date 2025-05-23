using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Common;
using Unity.UOS.COSXML.Model.Object;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.CosException;
using Unity.UOS.COSXML.Utils;

namespace Unity.UOS.COSXML.Model.CI
{
    /// <summary>
    /// 提交视频审核任务
    /// <see href="https://cloud.tencent.com/document/product/436/47316"/>
    /// </summary>
    public sealed class DescribeMediaBucketsRequest : CIRequest
    {
        public DescribeMediaBucketsRequest() : base()
        {
            this.method = CosRequestMethod.GET;
            this.SetRequestPath("/mediabucket");
        }

        public void SetRegions(string regions)
        {
            this.queryParameters.Add("regions", regions);
        }

        public void SetBucketNames(string bucketNames)
        {
            this.queryParameters.Add("bucketNames", bucketNames);
        }

        public void SetBucketName(string bucketName)
        {
            this.queryParameters.Add("bucketName", bucketName);
        }

        public void SetPageNumber(string pageNumber)
        {
            this.queryParameters.Add("pageNumber", pageNumber);
        }

        public void SetPageSize(string pageSize)
        {
            this.queryParameters.Add("pageSize", pageSize);
        }
    }
}
