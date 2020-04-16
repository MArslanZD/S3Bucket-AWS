using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeBackup.Core.Communication.Bucket;
using LifeBackup.Core.Communication.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LifeBackup.Controllers
{
    [Route("api/bucket")]
    [ApiController]
    public class BucketController : ControllerBase
    {
        private readonly IBucketRepository _bucketRepository;
        public BucketController(IBucketRepository bucketRepository) => _bucketRepository = bucketRepository;

        [HttpPost]
        [Route("create/{bucketName}")]
        public async Task<ActionResult<CreateBucketResponse>> CreateS3Bucket(string bucketName)
        {
            var bucketExists = await _bucketRepository.DoesS3BucketExist(bucketName);
            if (bucketExists)
                return BadRequest("S3 Bucket Exists");
            var result = await _bucketRepository.CreateBucket(bucketName);
            if (result.Equals(null))
                return BadRequest();
            return Ok(result);
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<ListS3BuckeetsResponse>>> ListS3Buckets()
        {
            var result = await _bucketRepository.ListBuckets();

            if (result.Equals(null))
                return NotFound();
            return Ok(result);
        }

        [HttpDelete]
        [Route("delete/{bucketName}")]
        public async Task<IActionResult> DeleteS3Bucket(string bucketName)
        {
            await _bucketRepository.DeleteBucket(bucketName);
            return Ok();
        }
    }
}