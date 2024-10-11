using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Data;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Models;
using TallerIDWMBackend.DTOs.Post;

namespace TallerIDWMBackend.Controllers
{
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly IPhotoService _photoService;

        public PostController(ApplicationDbContext dataContext, IPhotoService photoService){
            _dataContext = dataContext;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Post>>> GetPosts()
        {
            return await _dataContext.Posts.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreatePost(CreatePostDto createPostDto)
        {
            var result = await _photoService.AddPhotoAsync(createPostDto.Image);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            };

            var post = new Post
            {
                Title = createPostDto.Title,
                Description = createPostDto.Description,
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            await _dataContext.Posts.AddAsync(post);
            await _dataContext.SaveChangesAsync();

            return Ok();
        }
    }
}