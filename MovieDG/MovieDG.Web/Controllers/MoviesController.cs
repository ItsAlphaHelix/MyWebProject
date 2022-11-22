﻿namespace MovieDG.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MovieDG.Core.Contracts;
    using MovieDG.Core.ViewModels.Movies;

    public class MoviesController : Controller
    {
        private readonly IMovieService movieService;

        public MoviesController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        public async Task<IActionResult> All()
        {
            var movies = await this.movieService.GetAllMoviesAsync();

            return View(movies);
        }

        public async Task<IActionResult> Details(int id)
        { 
            var movies = await this.movieService.GetMovieDetailsAsync(id);

            return View(movies);
        }
    }
}
