using Common;
using Ruanmou.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCache
{
    /// <summary>
    /// 1 缓存是什么，各级缓存
    /// 2 本地缓存原理和实现
    /// 3 缓存应用和缓存更新
    /// 
    /// 缓存：为了快速获取结果，在第一次获取数据后存起来，下次直接用
    /// 
    /// 缓存究竟用在哪里？
    /// 1 会重复请求   
    /// 2 数据相对稳定
    /// 3 耗时/耗资源
    /// 4 体积不大
    /// 
    /// 配置文件；菜单-权限；省市区；类别数据；
    /// 热搜(二八原则)；公告；技能/属性；数据字典；
    /// 分页(只要数据不是经常变)
    /// 
    /// 如果一个数据缓存一次，能被有效查询4次，就是值得的(大型系统的时候，为了性能，为了压力，更多缓存)
    /// 
    /// 缓存本身是共享的，应该是唯一的
    /// 
    /// 本地缓存空间小，不能跨进程共享
    /// 小项目随意缓存
    /// 中大型不够用的，一般会用分布式缓存
    /// 
    /// Memcached:内存管理
    /// Redis：REmote DIctionary Server
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("欢迎来到.net高级班vip课程，今天是Eleven老师为大家带来的缓存Cache");
                {
                    ////for (int i = 0; i < 5; i++)
                    ////{
                    ////    Console.WriteLine($"获取{nameof(DBHelper)} {i}次 {DateTime.Now.ToString("yyyyMMdd HHmmss.fff")}");
                    ////    List<Program> programList = DBHelper.Query<Program>(123);
                    ////}
                    ////1 重复请求   2 参数稳定时，结果不变  3 耗时/耗资源
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    Console.WriteLine($"获取{nameof(DBHelper)} {i}次 {DateTime.Now.ToString("yyyyMMdd HHmmss.fff")}");
                    //    List<Program> programList = null;
                    //    string key = $"{nameof(DBHelper)}_Query_{nameof(Program)}_{123}";//1 能精确描述这次操作  2 能够重现
                    //    //if (!CustomCache.Exsit(key))
                    //    //{
                    //    //    programList = DBHelper.Query<Program>(123);
                    //    //    //CustomCache.SaveOrUpdate(key, programList);
                    //    //    CustomCache.Add(key, programList);
                    //    //}
                    //    //else
                    //    //{
                    //    //    programList = CustomCache.Get<List<Program>>(key);
                    //    //}
                    //    programList = CustomCache.Find<List<Program>>(key, () => DBHelper.Query<Program>(123));
                    //    Console.WriteLine($"{i}次获取的数据为{programList.GetHashCode()}");
                    //}
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    Console.WriteLine($"获取{nameof(FileHelper)} {i}次 {DateTime.Now.ToString("yyyyMMdd HHmmss.fff")}");
                    //    List<Program> programList = null;
                    //    string key = $"{nameof(FileHelper)}_Query_{nameof(Program)}_{234}";
                    //    //if (!CustomCache.Exsit(key))
                    //    //{
                    //    //    programList = FileHelper.Query<Program>(234);
                    //    //    CustomCache.Add(key, programList);
                    //    //}
                    //    //else
                    //    //{
                    //    //    programList = CustomCache.Get<List<Program>>(key);
                    //    //}
                    //    programList = CustomCache.Find<List<Program>>(key, () => FileHelper.Query<Program>(234));
                    //    Console.WriteLine($"{i}次获取的数据为{programList.GetHashCode()}");
                    //}
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    Console.WriteLine($"获取{nameof(RemoteHelper)} {i}次 {DateTime.Now.ToString("yyyyMMdd HHmmss.fff")}");
                    //    List<Program> programList = null;
                    //    string key = $"{nameof(RemoteHelper)}_Query_{nameof(Program)}_{345}";
                    //    //if (!CustomCache.Exsit(key))
                    //    //{
                    //    //    programList = RemoteHelper.Query<Program>(345);
                    //    //    CustomCache.Add(key, programList);
                    //    //}
                    //    //else
                    //    //{
                    //    //    programList = CustomCache.Get<List<Program>>(key);
                    //    //}
                    //    programList = CustomCache.Find<List<Program>>(key, () => { return RemoteHelper.Query<Program>(345); });
                    //    Console.WriteLine($"{i}次获取的数据为{programList.GetHashCode()}");
                    //}

                }
                {
                    ////缓存的效果是出来了，但是我有个疑问，这里的数据一直是用之前的，如果结果有变怎么办呢？
                    ////缓存一般是就会有延迟的，这个很难避免，因为缓存的本质决定了是使用上一次的结果

                    ////一个用户权限----用户-角色-菜单  查询比较麻烦，也很频繁，相对稳定  所以为每个用户缓存一个
                    //{
                    //    //1 假如修改了某个用户的角色，缓存数据的其实失效了,
                    //    //只对Eleven用户的权限数据失效，只有一条数据失效了  Remove
                    //    //Remove不是Update：缓存只是一个临时存储，不是数据源，也许更新了还用不上的
                    //    string key = "Eleven_Privage";
                    //    CustomCache.Add(key, "12356");
                    //    CustomCache.Remove(key);

                    //    CustomCache.Find<string>(key, () => "1234567");
                    //}
                    //{
                    //    //2 删除一个菜单，会影响一大批的用户
                    //    //菜单--角色--用户，然后拼装出全部的key，然后遍历删除？  不对！  成本太高，缓存应该是提示性能，而不是负担
                    //    //全部删除  还会误伤别的缓存，造成缓存穿透
                    //    CustomCache.RemoveAll();

                    //    //我想清除的是跟菜单有关的数据
                    //    //在key里面做文字，把缓存数据分类
                    //    string key = "_Eleven_Privage_Menu_";//只要缓存跟菜单有关的数据，都加上_Menu_
                    //    CustomCache.RemoveCondition(s => s.Contains("_Menu_"));
                    //    //我只想删除那一个id，不可能的，控制不了这么细致
                    //}
                    {
                        //定时作业更新了数据--远程接口更新了数据，更多的时候是我们不知道的情况下，数据变了
                        //给系统提供一个接口，更新数据也来通知下系统(能解决少量场景)
                        //缓存，是沿用上一次的结果，根本就不会去数据源的-----肯定有误差
                        //可以做一下过期，加一个时间有效性，  数据延迟来 换取性能，降低压力，需要抉择

                        //string key = "_Eleven_Privage_Menu_";
                        //if (CustomCache.Exsit(key))
                        //{
                        //    string result = CustomCache.Get<string>(key);
                        //}
                        //else
                        //{
                        //    string result = "23426758967979";
                        //    CustomCache.Add(key, result, 10);
                        //}

                        //if (CustomCache.Exsit(key))//只要在有效期内，不管数据源有没有变化，都以缓存为准
                        //{
                        //    string result = CustomCache.Get<string>(key);
                        //}
                        //else
                        //{
                        //    string result = "23426758967979";
                        //    CustomCache.Add(key, result, 5);
                        //}
                        //Thread.Sleep(5000);
                        //if (CustomCache.Exsit(key))//只要超过有效期，不管数据源有没有变化，都去重新获取
                        //{
                        //    string result = CustomCache.Get<string>(key);
                        //}
                        //else
                        //{
                        //    string result = "23426758967979";
                        //    CustomCache.Add(key, result, 5);
                        //}
                    }
                    {
                        List<Task> tasks = new List<Task>();
                        for (int i = 0; i < 1000; i++)
                        {
                            int k = i;
                            tasks.Add(Task.Run(() =>
                           {
                               CustomCache.Add($"TestKey_{k}", $"TestValue_{k}", new Random().Next(1000, 2000));
                           }));
                        }
                        //:“已添加了具有相同键的项。” 字典错误，相同的key  多线程临时变量
                        //未将对象引用设置到对象的实例。null问题 也应该是数组的问题，猜测是计算地址的时候，线程冲突

                        //Add时，数组问题：空间换性能，两个数组，多线程的时候出现
                        //:“目标数组的长度不够。请检查 destIndex 和长度以及数组的下限。”
                        //:“索引超出了数组界限。”

                        //“集合已修改；可能无法执行枚举操作。”

                        Task.WaitAll(tasks.ToArray());
                        string result = CustomCache.Get<string>("TestKey_0");
                    }
                    object a = new object();
                    object b = new object();

                    Task.Run(() =>
                    {
                        lock (a)
                        {
                            Console.WriteLine("获取了a的独占1");
                            Thread.Sleep(2000);
                            lock (b)
                            {
                                Console.WriteLine("获取了b的独占1");
                                Thread.Sleep(2000);
                            }
                        }
                    });
                    Task.Run(() =>
                    {
                        lock (b)
                        {
                            Console.WriteLine("获取了b的独占2");
                            Thread.Sleep(2000);
                            lock (a)
                            {
                                Console.WriteLine("获取了a的独占2");
                                Thread.Sleep(2000);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
