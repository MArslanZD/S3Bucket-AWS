using Amazon.S3;
using Amazon.S3.Model;
using LifeBackup.Core.Communication.Bucket;
using LifeBackup.Core.Communication.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeBackup.Infrastructure.Repositories
{
    public class BucketRepository : IBucketRepository
    {
        private IAmazonS3 _s3Client;
        public BucketRepository(IAmazonS3 s3Client) => _s3Client = s3Client;

        public async Task<CreateBucketResponse> CreateBucket(string bucketName)
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            };
            var response = await  _s3Client.PutBucketAsync(putBucketRequest);
            return new CreateBucketResponse
            {
                BucketName = bucketName,
                RequestId = response.ResponseMetadata.RequestId
            };
        }

        public async Task<IEnumerable<ListS3BuckeetsResponse>> ListBuckets()
        {
            var response = await _s3Client.ListBucketsAsync();
            return response.Buckets.Select(b => new ListS3BuckeetsResponse {
                BucketName = b.BucketName,
                CreationDate = b.CreationDate
            });
        }



        public async Task<bool> DoesS3BucketExist(string bucketName)
        {
            return await _s3Client.DoesS3BucketExistAsync(bucketName);
        }

        public async Task DeleteBucket(string bucketName)
        {
            await _s3Client.DeleteBucketAsync(bucketName);
        }
    }
}
