using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using shortid;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace URL_Shortner.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ShortController : ControllerBase
    {

        private readonly AppDbContext _dbContext;
        public ShortController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("shorten")]
        public shorturl shorten([FromBody] shorten_me longdata)
        {
            shorturl newurl = new shorturl();

            //check
            if (!longdata.long_url.StartsWith("http") || !longdata.long_url.Contains("://"))
            {
                throw new Exception("Invalid URL, url must start with 'http://' or 'https://'");
            }

            short_urls urlins = new short_urls();
            urlins.shortcode = generate_short_url(longdata.long_url);
            urlins.createdate = DateTime.Now;
            urlins.expiry_time = longdata.expiry_time == null || longdata.expiry_time == DateTime.MinValue ? DateTime.Now : longdata.expiry_time;
            urlins.long_url = longdata.long_url;
            _dbContext.short_urls.Add(urlins);
            _dbContext.SaveChanges();

            newurl.short_url = Request.Scheme + "://" + Request.Host + "/api/shorten/" + urlins.shortcode;

            return newurl;
        }

        [HttpGet]
        [Route("shorten/{urlcode}")]
        public async Task<IActionResult> GoTo(string urlcode)
        {
            //Check URL 
            IQueryable<short_urls> urlcheck = _dbContext.short_urls.Where(x => x.shortcode == urlcode);



            if (urlcheck != null || urlcheck.Count() > 0)
            {
                short_urls url = urlcheck.First();

                if (url.expiry_time == url.createdate || url.expiry_time < DateTime.Now)
                {
                    //check click count 
                    IQueryable<url_access> accessed = _dbContext.url_access.Where(x => x.urlid == url.urlid && x.accessdate >= DateTime.Now.AddMinutes(-1)).OrderByDescending(o => o.accessdate);

                    if (accessed != null && accessed.Count() > 10)
                        return new StatusCodeResult(429);
                    else
                    {
                        url_access acc = new url_access()
                        {
                            accessdate = DateTime.Now,
                            urlid = url.urlid
                        };

                        _dbContext.url_access.Add(acc);
                        _dbContext.SaveChanges();
                        return Redirect(url.long_url);
                    }
                }
            }

            return NotFound();


        }

        [HttpGet]
        [Route("shorten/{urlcode}/stats")]
        public async Task<IActionResult> GetStats(string urlcode)
        {
            url_stats stats = new url_stats();
            IQueryable<short_urls> urlcheck = _dbContext.short_urls.Where(x => x.shortcode == urlcode);



            if (urlcheck != null || urlcheck.Count() > 0)
            {
                short_urls url = urlcheck.First();


                //check click count 
                IQueryable<url_access> accessed = _dbContext.url_access.Where(x => x.urlid == url.urlid ).OrderByDescending(o => o.accessdate);
                if (urlcheck != null || urlcheck.Count() > 0)
                {
                    stats.click_count = accessed.Count();
                }
                else
                {
                    stats.click_count = 0;
                }


                return new JsonResult(stats);
            }

            return NotFound();
        }

        private string generate_short_url(string longurl)
        {
            return GenerateGUID();
        }

        private string GenerateGUID(int attempt = 1)
        {

            if (attempt > 4)
            {
                throw new Exception("Cannot create url , please try again");
            }



            JimRandom rand = new JimRandom();
            int length = rand.Next(8, 14);


            shortid.Configuration.GenerationOptions opts = new shortid.Configuration.GenerationOptions(true, false, length);
            string guid = ShortId.Generate();
            guid.Substring(0, 5);


            IQueryable<short_urls> urlcheck = _dbContext.short_urls.Where(x => x.shortcode == guid);



            if (urlcheck == null || urlcheck.Count() > 0)
            {
                return GenerateGUID(attempt++);
            }



            return guid;

        }
    }

    public class shorten_me
    {

        public string long_url { get; set; }

        [AllowNull]
        public DateTime expiry_time { get; set; }
    }

    public class shorturl
    {
        public string short_url { get; set; }
    }

    public class url_stats
    {
        public long click_count { get; set; }
    }




}
