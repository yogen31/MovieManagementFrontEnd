using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieManagementFrontEnd.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace MovieManagementFrontEnd.Controllers
{
    public class LandingPageController : Controller
    {
        public static string BaseUrl = "https://localhost:7146";
        private readonly UserManager<IdentityUser> _userManager;
        public LandingPageController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int limit, int offset, string searchName)
        {
            if (limit <= 0)
            {
                limit = 9;
            }
            if (offset <= 0)
            {
                offset = 0;
            }
            if (searchName == null)
            {
                searchName = "";
            }
            MovieRequestViewModel movieRequestViewModel = new MovieRequestViewModel();
            movieRequestViewModel.Limit = limit;
            movieRequestViewModel.Offset = offset;
            movieRequestViewModel.SearchName = searchName;

            string apiUrl = BaseUrl + "/api/movie/allmovies";
            string queryString = $"?limit={movieRequestViewModel.Limit}&offset={movieRequestViewModel.Offset}" +
                $"&searchName={Uri.EscapeDataString(movieRequestViewModel.SearchName)}";

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl + queryString);
            var responseString = response.Content.ReadAsStringAsync();
            List<MovieViewModel>? movies = new List<MovieViewModel>();
            if (response.IsSuccessStatusCode)
            {
                movies = JsonConvert.DeserializeObject<List<MovieViewModel>>(responseString.Result);
            }
            ViewBag.Search = movieRequestViewModel.SearchName;
            return View(movies);
        }
        public async Task<IActionResult> Favourite(int limit, int offset, string searchName)
        {
            if (limit <= 0)
            {
                limit = 9;
            }
            if (offset <= 0)
            {
                offset = 0;
            }
            if (searchName == null)
            {
                searchName = "";
            }
            MovieRequestViewModel movieRequestViewModel = new MovieRequestViewModel();
            movieRequestViewModel.Limit = limit;
            movieRequestViewModel.Offset = offset;
            movieRequestViewModel.SearchName = searchName;
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                movieRequestViewModel.UserId = user?.Id;
            }
            string apiUrl = BaseUrl + "/api/movie/getuserfavourite";
            string queryString = $"?limit={movieRequestViewModel.Limit}&offset={movieRequestViewModel.Offset}" +
                $"&searchName={Uri.EscapeDataString(movieRequestViewModel.SearchName)}&UserId={movieRequestViewModel.UserId}";

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl + queryString);
            var responseString = response.Content.ReadAsStringAsync();
            List<MovieViewModel>? movies = new List<MovieViewModel>();
            if (response.IsSuccessStatusCode)
            {
                movies = JsonConvert.DeserializeObject<List<MovieViewModel>>(responseString.Result);
            }
            ViewBag.Search = movieRequestViewModel.SearchName;
            return View(movies);
        }
        //public async Task<List<MovieViewModel>> GetMovieData(int limit, int offset, string searchName)
        //{
        //    if (limit <= 0)
        //    {
        //        limit = 9;
        //    }
        //    if (offset <= 0)
        //    {
        //        offset = 0;
        //    }
        //    if (searchName == null)
        //    {
        //        searchName = "";
        //    }
        //    MovieRequestViewModel movieRequestViewModel = new MovieRequestViewModel();
        //    movieRequestViewModel.Limit = limit;
        //    movieRequestViewModel.Offset = offset;
        //    movieRequestViewModel.SearchName = searchName;

        //    string apiUrl = BaseUrl + "/api/movie/allmovies";
        //    string queryString = $"?limit={movieRequestViewModel.Limit}&offset={movieRequestViewModel.Offset}" +
        //        $"&searchName={Uri.EscapeDataString(movieRequestViewModel.SearchName)}";

        //    HttpClient httpClient = new HttpClient();
        //    var response = await httpClient.GetAsync(apiUrl + queryString);
        //    var responseString = response.Content.ReadAsStringAsync();
        //    List<MovieViewModel>? movies = new List<MovieViewModel>();
        //    if (response.IsSuccessStatusCode)
        //    {
        //        movies = JsonConvert.DeserializeObject<List<MovieViewModel>>(responseString.Result);
        //    }
        //    return movies;
        //}
        //[HttpPost]
        //public async Task<IActionResult> UpdateData(int limit, int offset, string searchName)
        //{
        //    // Call the necessary logic to retrieve the updated data based on the search query and parameters
        //    List<MovieViewModel> updatedData = await GetMovieData(limit, offset, searchName);

        //    return Json(updatedData);
        //}
    }
}
