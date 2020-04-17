using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeBackup.Core.Communication.Files;
using LifeBackup.Core.Communication.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LifeBackup.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;
        public FilesController(IFileRepository fileRepository) => _fileRepository = fileRepository;


        [HttpPost]
        [Route("{bucketName}/add")]
        public async Task<ActionResult<AddFileResponse>> AddFiles(string bucketName, IList<IFormFile> formFiles)
        {
            if (formFiles.Equals(null))
                return BadRequest("The request doesn't contain any files to be uploaded.");
            var response = await _fileRepository.UploadFiles(bucketName, formFiles);
            if (response.Equals(null))
                return BadRequest();
            return Ok(response);
        }

        [HttpGet]
        [Route("{bucketName}/list")]
        public async Task<ActionResult<IEnumerable<ListFilesResponse>>> ListFiles(string bucketName)
        {
            var response = await _fileRepository.ListFiles(bucketName);            
            return Ok(response);
        }

        [HttpGet]
        [Route("{bucketName}/download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string bucketName, string fileName)
        {
            await _fileRepository.DownloadFile(bucketName,fileName);
            return Ok();
        }

        [HttpDelete]
        [Route("{bucketName}/delete/{fileName}")]
        public async Task<ActionResult<DeleteFileResponse>> DeleteFile(string bucketName, string fileName)
        {
            await _fileRepository.DeleteFile(bucketName, fileName);
            return Ok();
        }


        [HttpPost]
        [Route("{bucketName}/addjsonobject")]
        public async Task<IActionResult> AddJsonObject(string bucketName, AddJsonObjectRequest request)
        {
            await _fileRepository.AddJsonObject(bucketName,request);            
            return Ok();
        }


        [HttpGet]
        [Route("{bucketName}/getjsonobject}")]
        public async Task<ActionResult<GetJsonObjectResponse>> GetJsonObject(string bucketName, string fileName)
        {
            var response = await _fileRepository.GetJsonObject(bucketName, fileName);
            return Ok(response);
        }

    }
}