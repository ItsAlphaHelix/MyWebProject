namespace MovieDG.Core.Services
{
    using Microsoft.EntityFrameworkCore;
    using MovieDG.Core.Contracts;
    using MovieDG.Core.ViewModels.Actors;
    using MovieDG.Core.ViewModels.Countries;
    using MovieDG.Core.ViewModels.Genres;
    using MovieDG.Core.ViewModels.Movies;
    using MovieDG.Data.Data.Models;
    using MoviesDG.Data.Models;
    using MoviesDG.Data.Repositories;

    public class MovieService : IMovieService
    {
        private readonly IRepository<Movie> moviesRepository;
        private readonly IRepository<ApplicationUser> userRepository;
        public MovieService(
            IRepository<Movie> moviesRepository,
            IRepository<ApplicationUser> userRepository
            )
        {
            this.moviesRepository = moviesRepository;
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<MovieViewModel>> GetAllMoviesAsync()
        {
            var movies = await moviesRepository
                .AllAsNoTracking()
                .Select(x => new MovieViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    Trailer = x.Trailer,
                    Popularity = x.Popularity,
                    AverageVotes = x.AverageVotes
                })
                .ToListAsync();

            return movies;
        }

        public async Task<MovieDetailsViewModel> GetMovieDetailsAsync(int id)
        {
            var movie = await moviesRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new MovieDetailsViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    Runtime = x.Runtime,
                    ReleaseDate = x.ReleaseDate,
                    Overview = x.Overview,
                    Trailer = x.Trailer,
                    Genres = x.MovieGenres.Select(x => new GenreViewModel()
                    {
                        Name = x.Genre.Type
                    }),
                    Actors = x.MovieActors.Select(x => new ActorViewModel()
                    {
                        Name = x.Actor.Name,
                        CharacterName = x.CharacterName
                    }),
                    Countries = x.MovieCountries.Select(x => new CountryViewModel()
                    {
                        Name = x.Country.Name
                    })
                })
                .FirstOrDefaultAsync();

            if (movie == null)
            {
                throw new ArgumentNullException($"The ${movie} can not be null");
            }

            return movie;

        }

        public async Task<IEnumerable<MovieViewModel>> GetTopRatedMoviesAsync()
        {
            var topRatedMovies = await this.moviesRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.AverageVotes)
                .Select(x => new MovieViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    Trailer = x.Trailer,
                    Popularity = x.Popularity,
                    AverageVotes = x.AverageVotes
                })
                .Take(10)
                .ToListAsync();

            return topRatedMovies;
        }

        public async Task<IEnumerable<MovieViewModel>> GetPopularityMoviesAsync()
        {
            var popularityMovies = await this.moviesRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.Popularity)
                .Select(x => new MovieViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    Trailer = x.Trailer,
                    Popularity = x.Popularity,
                    AverageVotes = x.AverageVotes
                })
                .Take(10)
                .ToListAsync();

            return popularityMovies;
        }

        public async Task<IEnumerable<MovieViewModel>> GetRecentMoviesAsync()
        {
            var recentMovies = await this.moviesRepository
                 .AllAsNoTracking()
                 .OrderByDescending(x => x.ReleaseDate)
                 .Select(x => new MovieViewModel()
                 {
                     Id = x.Id,
                     Title = x.Title,
                     Poster = x.Poster,
                     Trailer = x.Trailer,
                     Popularity = x.Popularity,
                     AverageVotes = x.AverageVotes
                 })
                 .Take(10)
                 .ToListAsync();

            return recentMovies;
        }

        public async Task<BannerHomeMovieViewModel> GetLatestMovieAsync()
        {
            var latestMovie = await this.moviesRepository
                 .AllAsNoTracking()
                 .OrderByDescending(x => x.ReleaseDate)
                 .Select(x => new BannerHomeMovieViewModel()
                 {
                     Title = x.Title,
                     Trailer = x.Trailer,
                     Banner = x.Banner,
                     Runtime = x.Runtime,
                     ReleaseDate = x.ReleaseDate,
                     Genres = x.MovieGenres.Select(x => new GenreViewModel()
                     {
                         Name = x.Genre.Type
                     }),
                 })
                 .FirstOrDefaultAsync();

            if (latestMovie == null)
            {
                throw new ArgumentNullException($"The ${latestMovie} can not be null");
            }

            return latestMovie;
        }

        public async Task<IEnumerable<MovieViewModel>> GetMoviesByGenreAsync(string name)
        {
            var movies = await this.moviesRepository
                .AllAsNoTracking()
                .Where(x => x.MovieGenres.Any(x => x.Genre.Type == name))
                .OrderByDescending(x => x.ReleaseDate.Year)
                .Select(x => new MovieViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    Trailer = x.Trailer,
                    AverageVotes = x.AverageVotes
                })
                .ToListAsync();

            return movies;
        }

        public async Task<IEnumerable<MovieViewModel>> GetMoviesByCountryAsync(string name)
        {
            var movies = await this.moviesRepository
                .AllAsNoTracking()
                .Where(x => x.MovieCountries.Any(x => x.Country.Name == name))
                .OrderByDescending(x => x.ReleaseDate.Year)
                .Select(x => new MovieViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    Trailer = x.Trailer,
                    AverageVotes = x.AverageVotes
                })
                .ToListAsync();

            return movies;
        }

        public async Task<IEnumerable<MovieViewModel>> GetMoviesByActorAsync(string name)
        {
            var movies = await this.moviesRepository
                .AllAsNoTracking()
                .Where(x => x.MovieActors.Any(x => x.Actor.Name == name))
                .OrderByDescending(x => x.ReleaseDate.Year)
                .Select(x => new MovieViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    Trailer = x.Trailer,
                    AverageVotes = x.AverageVotes
                })
                .ToListAsync();

            return movies;
        }

        public async Task AddMovieToCollectionAsync(int movieId, string userId)
        {
            var user = await userRepository
               .All()
               .Where(u => u.Id == userId)
               .Include(u => u.UsersMovies)
               .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            var movie = await moviesRepository
                        .All()
                        .FirstOrDefaultAsync(u => u.Id == movieId);
            if (movie == null)
            {
                throw new ArgumentException("Invalid Movie ID");
            }

            if (!user.UsersMovies.Any(m => m.MovieId == movieId))
            {
                user.UsersMovies.Add(new UserMovie()
                {
                    MovieId = movie.Id,
                    UserId = user.Id,
                    Movie = movie,
                    User = user
                });
            }
            else
            {
                var userMovie = movie.UsersMovies.FirstOrDefault(x => x.MovieId == movieId);

                userMovie.IsActive = true;
            }

            await userRepository.SaveChangesAsync();
        }
        public async Task<IEnumerable<MovieViewModel>> GetAllMyMoviesAsync(string userId)
        {
            var user = await userRepository
                .AllAsNoTracking()
                .Where(u => u.Id == userId)
                .Include(u => u.UsersMovies)
                .ThenInclude(x => x.Movie)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            var userMovies = user.UsersMovies
                .Where(x => x.IsActive == true)
                .Select(x => new MovieViewModel()
                {
                    Id = x.MovieId,
                    Title = x.Movie.Title,
                    Poster = x.Movie.Poster,
                    Trailer = x.Movie.Trailer,
                    Popularity = x.Movie.Popularity,
                    AverageVotes = x.Movie.AverageVotes
                })
                .ToList();

            return userMovies;
        }

        public async Task RemoveMovieFromCollectionAsync(int movieId, string userId)
        {
            var user = await userRepository
              .All()
              .Where(u => u.Id == userId)
              .Include(u => u.UsersMovies)
              .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            var movie = user.UsersMovies.FirstOrDefault(m => m.MovieId == movieId);

            if (movie != null)
            {
                movie.IsActive = false;

                await this.userRepository.SaveChangesAsync();
            }
        }

        public async Task RemoveAllMoviesFromCollectionAsync(int movieId, string userId)
        {
            var user = await userRepository
              .All()
              .Where(u => u.Id == userId)
              .Include(u => u.UsersMovies)
              .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            foreach (var userMovie in user.UsersMovies)
            {
                userMovie.IsActive = false;
            }

            await this.userRepository.SaveChangesAsync();
        }
    }
}
