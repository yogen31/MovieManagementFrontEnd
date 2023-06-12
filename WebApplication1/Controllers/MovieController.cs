using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieManagementFrontEnd.ViewModels;
using Newtonsoft.Json;
using System.Text;
using WebApplication1.Data;

namespace MovieManagementFrontEnd.Controllers
{
    public class MovieController : Controller
    {
        //public static int MovieIdCheck = 0;
        private readonly UserManager<IdentityUser> _userManager;
        public static string BaseUrl = "https://localhost:7146";

        public MovieController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddMovies(MovieViewModel movieViewModel)
        {
            if (movieViewModel == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                MovieMapViewModel movieMapViewModel = new MovieMapViewModel();
                movieMapViewModel.Title = movieViewModel.Title;
                movieMapViewModel.Description = movieViewModel.Description;
                movieMapViewModel.Genre = movieViewModel.Genre;
                movieMapViewModel.MovieLink = movieViewModel.MovieLink;
                if (movieViewModel.FormFile != null)
                {
                    if (movieViewModel.FormFile != null && movieViewModel.FormFile.Length > 0)
                    {
                        //get file name 
                        var fileName = Path.GetFileName(movieViewModel.FormFile.FileName);
                        var combinePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads",
            movieViewModel.FormFile.FileName);
                        using (var stream = new FileStream(combinePath, FileMode.Create))
                        {
                            movieViewModel.FormFile.CopyTo(stream);
                        }
                        //save the file to database
                        movieMapViewModel.MediaPath = "/uploads/" + movieViewModel.FormFile.FileName;
                    }
                }
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    movieMapViewModel.UserId = user?.Id;
                }
                HttpClient httpClient = new HttpClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(movieMapViewModel), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(BaseUrl+"/api/movie/addmovie", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    // Request was successful
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("Index", "LandingPage");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("Index");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddReview(int MovieId, string ReviewComment)
        {
            MovieReviewViewModel movieReviewViewModel = new MovieReviewViewModel();
            movieReviewViewModel.MovieId = MovieId;
            movieReviewViewModel.ReviewComment= ReviewComment;
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                movieReviewViewModel.UserId = user?.Id;
            }
            HttpClient httpClient = new HttpClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(movieReviewViewModel), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseUrl+"/api/movie/addreview", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                string responseBody = await response.Content.ReadAsStringAsync();
                //MovieIdCheck = movieReviewViewModel.MovieId;
                return RedirectToAction("ViewMovieById");
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                return RedirectToAction("ViewMovieById?id=" + movieReviewViewModel.MovieId,"Movie");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddUserFavourite(int MovieId,int isFavourite)
        {

            UserFavouriteViewModel userFavouriteViewModel = new UserFavouriteViewModel();
            userFavouriteViewModel.MovieId = MovieId;
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                userFavouriteViewModel.UserId = user?.Id;
            }
            HttpClient httpClient = new HttpClient();
            if (isFavourite == 1)
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(userFavouriteViewModel), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(BaseUrl + "/api/movie/adduserfavourite", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    // Request was successful
                    string responseBody = await response.Content.ReadAsStringAsync();
                    //MovieIdCheck = userFavouriteViewModel.MovieId;
                    return RedirectToAction("ViewMovieById");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("ViewMovieById?id=" + userFavouriteViewModel.MovieId, "Movie");
                }
            }
            else
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(userFavouriteViewModel), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(BaseUrl + "/api/movie/removeuserfavourite", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    // Request was successful
                    string responseBody = await response.Content.ReadAsStringAsync();
                    //MovieIdCheck = userFavouriteViewModel.MovieId;
                    return RedirectToAction("ViewMovieById");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("ViewMovieById?id=" + userFavouriteViewModel.MovieId, "Movie");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddRating(int MovieId, int Rating)
        {
            MovieRatingViewModel movieRatingViewModel = new MovieRatingViewModel();
            movieRatingViewModel.MovieId = MovieId;
            movieRatingViewModel.Rating = Rating;
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                movieRatingViewModel.UserId = user?.Id;
            }
            HttpClient httpClient = new HttpClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(movieRatingViewModel), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseUrl+"/api/movie/addrating", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                string responseBody = await response.Content.ReadAsStringAsync();
                //MovieIdCheck = movieRatingViewModel.MovieId;
                return RedirectToAction("ViewMovieById");
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                //MovieIdCheck = movieRatingViewModel.MovieId;
                return RedirectToAction("ViewMovieById");
            }
        }
        public async Task<IActionResult> ViewMovieById(int id)
        {
            //if (MovieIdCheck != 0)
            //{
            //    id=MovieIdCheck;
            //}
            string? userId = null;
            var user1 = await _userManager.GetUserAsync(User);
            if (user1 != null)
            {
                userId = user1?.Id;
            }
            if (id != 0)
            {
                string apiUrl = BaseUrl+"/api/movie";
                string queryString = $"?id={id}&userId={userId}";

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync(apiUrl + queryString);
                var responseString = response.Content.ReadAsStringAsync();
                MovieViewModel? movie = new MovieViewModel();
                if (response.IsSuccessStatusCode)
                {
                    movie = JsonConvert.DeserializeObject<MovieViewModel>(responseString.Result);
                    if (movie.MovieReviews != null)
                    {
                        foreach(var item in movie.MovieReviews)
                        {
                            var user= await _userManager.FindByIdAsync(item.UserId);
                            if(user!=null)
                            {
                                item.UserName = user.UserName;

                            }
                            else
                            {
                                item.UserName = "Unknown User";
                            }
                        }
                    }
                }
                return View(movie);
            }
            else
            {
                return RedirectToAction("Index","LandingPage");
            }
            
        }
    }
}
