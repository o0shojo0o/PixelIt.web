using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelIT.web
{
    public class PixelItRepo
    {
        private MySqlConnection mysqlCon;

        public PixelItRepo(MySqlConnection mysqlCon)
        {
            this.mysqlCon = mysqlCon;
        }

        public List<PixelItBMP> GetBMPAll()
        {
            var result = mysqlCon.Query<PixelItBMP>(@"select 
                                                        a.*, 
                                                        b.name as username,
                                                        IFNULL(c.hitcount,0) as hitcount,
                                                        b.aktiv 
                                                    from pixel_it_bitmap a 
                                                    join pixel_it_user b on (a.userid  = b.id)
                                                    left outer join pixel_it_hitcount c on (a.id = c.pixel_id)").ToList();
            return result.OrderBy(x => x.Name).ThenBy(x => x.HitCount).ToList();
        }

        public PixelItBMP GetBMPByID(int bmpID)
        {
            SetHitCountBMP(bmpID);

            var result = mysqlCon.Query<PixelItBMP>(@"select 
                                                        a.*, 
                                                        b.name as username,
                                                        IFNULL(c.hitcount,0) as hitcount,
                                                        b.aktiv 
                                                    from pixel_it_bitmap a 
                                                    join pixel_it_user b on (a.userid  = b.id)
                                                    left outer join pixel_it_hitcount c on (a.id = c.pixel_id) 
                                                    where
                                                        a.id = @id", new { id = bmpID }).FirstOrDefault();
            return result;
        }

        public PixelItBMP GetBMPNewst()
        {
            var result = mysqlCon.Query<PixelItBMP>(@"select 
                                                        a.*, 
                                                        b.name as username,
                                                        IFNULL(c.hitcount,0) as hitcount,
                                                        b.aktiv 
                                                    from pixel_it_bitmap a 
                                                    join pixel_it_user b on (a.userid  = b.id)
                                                    left outer join pixel_it_hitcount c on (a.id = c.pixel_id) 
                                                    where
                                                        a.id = (select max(id) from pixel_it_bitmap)").FirstOrDefault();
            return result;
        }

        public List<PixelItBMP> GetBMPByTyp(BitmapType type)
        {
            if (type == BitmapType.Both)
            {
                return GetBMPAll();
            }

            var result = mysqlCon.Query<PixelItBMP>(@"select 
                                                        a.*, 
                                                        b.name as username,
                                                        IFNULL(c.hitcount,0) as hitcount,
                                                        b.aktiv 
                                                    from pixel_it_bitmap a 
                                                    join pixel_it_user b on (a.userid  = b.id)
                                                    left outer join pixel_it_hitcount c on (a.id = c.pixel_id) 
                                                    where
                                                        a.animated = @type", new { type = (type == BitmapType.Animated ? 1 : 0) }).ToList();

            return result.OrderBy(x => x.Name).ThenBy(x => x.HitCount).ToList();
        }

        public List<PixelItBMP> GetBMPBySearch(string searchText, BitmapType type)
        {
            searchText = $"%{searchText.ToLower()}%";
            var result = new List<PixelItBMP>();

            if (type == BitmapType.Both)
            {
                result = mysqlCon.Query<PixelItBMP>(@"select 
                                                        a.*, 
                                                        b.name as username,
                                                        IFNULL(c.hitcount,0) as hitcount,
                                                        b.aktiv 
                                                    from pixel_it_bitmap a 
                                                    join pixel_it_user b on (a.userid  = b.id)
                                                    left outer join pixel_it_hitcount c on (a.id = c.pixel_id) 
                                                    where
                                                        (LOWER(a.id) like @search or LOWER(a.name) like @search or LOWER(b.name) like @search)", new { search = searchText }).ToList();
            }
            else
            {
                result = mysqlCon.Query<PixelItBMP>(@"select 
                                                    a.*, 
                                                    b.name as username,
                                                    IFNULL(c.hitcount,0) as hitcount,
                                                    b.aktiv 
                                                from pixel_it_bitmap a 
                                                join pixel_it_user b on (a.userid  = b.id)
                                                left outer join pixel_it_hitcount c on (a.id = c.pixel_id) 
                                                where
                                                    a.animated = @type and (LOWER(a.id) like @search or LOWER(a.name) like @search or LOWER(b.name) like @search)", new { type = (type == BitmapType.Animated ? 1 : 0), search = searchText }).ToList();
            }

            return result.OrderBy(x => x.Name).ThenBy(x => x.HitCount).ToList();
        }

        public List<PixelItBMP> GetBMPByIDOffset(int startID, int results)
        {
            var result = mysqlCon.Query<PixelItBMP>(@"select 
                                                        a.*, 
                                                        b.name as username,
                                                        IFNULL(c.hitcount,0) as hitcount,
                                                        b.aktiv 
                                                    from pixel_it_bitmap a 
                                                    join pixel_it_user b on (a.userid  = b.id)
                                                    left outer join pixel_it_hitcount c on (a.id = c.pixel_id) 
                                                    where
                                                        a.id >= @id limit @limit", new { id = startID, limit = results }).ToList();

            return result.OrderBy(x => x.Name).ThenBy(x => x.HitCount).ToList();
        }

        public List<PixelItBMP> GetBMPsByUserID(int userID)
        {
            var result = mysqlCon.Query<PixelItBMP>(@"select 
                                                        a.*, 
                                                        b.name as username,
                                                        IFNULL(c.hitcount,0) as hitcount,
                                                        b.aktiv 
                                                    from pixel_it_bitmap a 
                                                    join pixel_it_user b on (a.userid  = b.id)
                                                    left outer join pixel_it_hitcount c on (a.id = c.pixel_id) 
                                                    where
                                                        a.userID = @userID", new { userID = userID }).ToList();

            return result.OrderBy(x => x.Name).ThenBy(x => x.HitCount).ToList();
        }

        public void SaveBMP(PixelItBMP bmp)
        {
            if (!IsUserNameValid(bmp.Username))
            {
                bmp.UserID = CreateNewUser(bmp.Username).ID;
            }
            else
            {
                bmp.UserID = GetUserByName(bmp.Username).ID;

            }


            mysqlCon.Execute("insert into pixel_it_bitmap (datetime, name, rgb565array, sizex, sizey, userid, animated) VALUES (@datetime, @name, @rgb565array, @sizex, @sizey, @userid,@animated)", new
            {
                datetime = DateTime.Now,
                name = bmp.Name,
                rgb565array = bmp.RGB565Array,
                sizex = bmp.SizeX,
                sizey = bmp.SizeY,
                userid = bmp.UserID,
                animated = bmp.Animated
            });


        }

        public bool IsUserNameValid(string userName)
        {
            var result = mysqlCon.Query<int>("Select count(*) from pixel_it_user where name = @name and aktiv = true", new { name = userName }).FirstOrDefault();

            if (result > 0)
                return true;
            else
                return false;
        }

        public PixelItUser GetUserByName(string userName)
        {
            var result = mysqlCon.Query<PixelItUser>("Select * from pixel_it_user where name = @name and aktiv = true", new { name = userName }).FirstOrDefault();

            return result;
        }

        public PixelItUser CreateNewUser(string userName)
        {
            var result = mysqlCon.Execute("insert into pixel_it_user (name, aktiv) VALUES (@name, @aktiv)", new { name = userName, aktiv = true });

            return GetUserByName(userName);
        }

        private void SetHitCountBMP(int pixelID)
        {
            var result = mysqlCon.Query<int>("Select count(*) from pixel_it_hitcount where pixel_id = @pixel_id", new { pixel_id = pixelID }).FirstOrDefault();

            if (result > 0)
            {
                mysqlCon.Execute("update pixel_it_hitcount set hitcount = hitcount +1 where pixel_id = @pixel_id", new { pixel_id = pixelID });
            }
            else
            {
                mysqlCon.Execute("insert into pixel_it_hitcount (pixel_id, hitcount) VALUES (@pixel_id, @hitcount)", new { pixel_id = pixelID, hitcount = 1 });
            }
        }
    }
}