using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DNWS
{
    public class TwitterApiPlugin : TwitterPlugin
    {

        private List<User> GetUser()
        {
            using (var context = new TweetContext())
            {
                try
                {
                    List<User> users = context.Users.Where(b => true).Include(b => b.Following).ToList();
                    return users;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }


        public override HTTPResponse GetResponse(HTTPRequest request)
        {
            HTTPResponse response = new HTTPResponse(200);

            string user = request.getRequestByKey("user");
            string password = request.getRequestByKey("password");
            string Follow = request.getRequestByKey("following");
            string followtimeline = request.getRequestByKey("timeline");
            string message = request.getRequestByKey("message");
            string[] path = request.Filename.Split("?"); //teach dy 600611030

            if (path[0] == "User")
            {
                if (request.Method == "GET")
                {
                    string y = JsonConvert.SerializeObject(GetUser());
                    response.body = Encoding.UTF8.GetBytes(y);
                }
                if (request.Method == "POST")
                {
                    Twitter.AddUser(user, password);
                    response.body = Encoding.UTF8.GetBytes("Add user success");
                }
                if (request.Method == "DELETE")
                {
                    Twitter.DeleteUser(user, password);
                    response.body = Encoding.UTF8.GetBytes("Delete user success");
                }

            }
            else if (path[0] == "following")
            {
                //Twitter twitter = new Twitter(user);
                if (request.Method == "GET")
                {
                    Twitter twitters = new Twitter(user);
                    string js = JsonConvert.SerializeObject(twitters.GetFollowing());
                    response.body = Encoding.UTF8.GetBytes(js);
                }
                else if (request.Method == "POST")
                {
                    if (Twitter.Check_User(Follow))
                    {
                        Twitter twitter = new Twitter(user);
                        twitter.AddFollowing(Follow);
                        response.body = Encoding.UTF8.GetBytes("200 OK");
                    }
                    else
                    {
                        response.status = 404;
                        response.body = Encoding.UTF8.GetBytes("404 User not exists");
                    }

                }
                else if (request.Method == "DELETE")
                {
                    try
                    {
                        Twitter twitter = new Twitter(user);
                        twitter.RemoveFollowing(Follow);
                        response.body = Encoding.UTF8.GetBytes("200 OK");
                    }
                    catch (Exception)
                    {
                        response.status = 404;
                        response.body = Encoding.UTF8.GetBytes("404 User not exists");
                    }
                }
            }

            else if (request.Method == "DELETE")
            {
                try
                {
                    Twitter twitter = new Twitter(user);
                    twitter.RemoveFollowing(Follow);
                    response.body = Encoding.UTF8.GetBytes("200 OK");
                }
                catch (Exception)
                {
                    response.status = 404;
                    response.body = Encoding.UTF8.GetBytes("404 User not exists");
                }
            }
        
            else if (path[0] == "Tweett")
            {
                Twitter twitter = new Twitter(user);
                if (request.Method == "GET")
                {
                    try
                    {
                        string timeline = request.getRequestByKey("timeline");
                        if (timeline == "following")
                        {
                            string json = JsonConvert.SerializeObject(twitter.GetFollowingTimeline());
                            response.body = Encoding.UTF8.GetBytes(json);
                        }
                        else
                        {
                            string json = JsonConvert.SerializeObject(twitter.GetUserTimeline());
                            response.body = Encoding.UTF8.GetBytes(json);
                        }
                    }
                    catch (Exception)
                    {
                        response.status = 404;
                        response.body = Encoding.UTF8.GetBytes("404 User not found");
                    }
                }
                else if (request.Method == "POST")
                {
                    try
                    {
                        twitter.PostTweet(message);
                        response.body = Encoding.UTF8.GetBytes("200 OK");
                    }
                    catch (Exception)
                    {
                        response.status = 404;
                        response.body = Encoding.UTF8.GetBytes("404 User not found");
                    }
                }
            }

            return response;
        }

    }
}


