using System;
using HarSharp;
using NUnit.Framework;

namespace VisualLogin.HarSharp.UnitTests
{
    [TestFixture]
    public class HarConvertTest
    {
        private Har m_actual;

        [SetUp]
        public void InitializeFixture()
        {
            m_actual = HarConvert.DeserializeFromFile(@"Hars\Sample.har");
        }

        [Test]
        public void Deserialize_Null_Exception()
        {
            Assert.Catch<ArgumentNullException>(() => HarConvert.Deserialize(null));
        }

        [Test]
        public void Deserialize_HarContent_Log()
        {
            var log = m_actual.Log;
            Assert.That("1.2", Is.EqualTo(log.Version));

            var creator = log.Creator;
            Assert.That("WebInspector", Is.EqualTo(creator.Name));
            Assert.That("537.36", Is.EqualTo(creator.Version));

            var browser = log.Browser;
            Assert.That(browser, Is.Null);

            var pages = log.Pages;

            Assert.That(3, Is.EqualTo(pages.Count));

            Assert.That("used by unit test", Is.EqualTo(log.Comment));
        }

        [Test]
        public void Deserialize_HarContent_Creator()
        {
            var creator = m_actual.Log.Creator;
            Assert.That("WebInspector", Is.EqualTo(creator.Name));
            Assert.That("537.36", Is.EqualTo(creator.Version));
        }

        [Test]
        public void Deserialize_HarContent_Browser()
        {
            var browser = m_actual.Log.Browser;
            Assert.That(browser, Is.Null);
        }

        [Test]
        public void Deserialize_HarContent_Pages()
        {
            var pages = m_actual.Log.Pages;
            Assert.That(3, Is.EqualTo(pages.Count));

            var page = pages[0];
            Assert.That(new DateTime(2014, 9, 24, 18, 39, 52, 160, DateTimeKind.Utc), Is.EqualTo(page.StartedDateTime));
            Assert.That("page_2", Is.EqualTo(page.Id));
            Assert.That("https://www.google.com/", Is.EqualTo(page.Title));
            Assert.That(2423.999786376953, Is.EqualTo(page.PageTimings.OnContentLoad));
            Assert.That(2423.999786376953, Is.EqualTo(page.PageTimings.OnLoad));

            page = pages[1];
            Assert.That(page.PageTimings.OnContentLoad, Is.Null);
            Assert.That(page.PageTimings.OnLoad, Is.Null);
        }

        [Test]
        public void Deserialize_HarContent_Entries()
        {
            var entries = m_actual.Log.Entries;
            Assert.That(60, Is.EqualTo(entries.Count));

            var entry = entries[0];
            Assert.That(new DateTime(2014, 9, 24, 18, 39, 52, 160, DateTimeKind.Utc),Is.EqualTo(entry.StartedDateTime));
            Assert.That(946.9997882843018, Is.EqualTo(entry.Time));
            Assert.That("134139", Is.EqualTo(entry.Connection));
            Assert.That("page_2", Is.EqualTo(entry.PageRef));
            Assert.That(entry.ServerIPAddress, Is.Null);
            Assert.That(0, Is.EqualTo(entry.Timings.Blocked));
            Assert.That(0, Is.EqualTo(entry.Timings.Dns));
            Assert.That(720.0000000011642, Is.EqualTo(entry.Timings.Connect));
            Assert.That(0, Is.EqualTo(entry.Timings.Send));
            Assert.That(225.99999999874854, Is.EqualTo(entry.Timings.Wait));
            Assert.That(0.9997882843890693, Is.EqualTo(entry.Timings.Receive));
            Assert.That(548.0000000025029, Is.EqualTo(entry.Timings.Ssl));
        }

        [Test]
        public void Deserialize_HarContent_Timings()
        {
            var timings = m_actual.Log.Entries[0].Timings;

            Assert.That(0, Is.EqualTo(timings.Blocked));
            Assert.That(0, Is.EqualTo(timings.Dns));
            Assert.That(720.0000000011642, Is.EqualTo(timings.Connect));
            Assert.That(0, Is.EqualTo(timings.Send));
            Assert.That(225.99999999874854, Is.EqualTo(timings.Wait));
            Assert.That(0.9997882843890693, Is.EqualTo(timings.Receive));
            Assert.That(548.0000000025029, Is.EqualTo(timings.Ssl));

            timings = m_actual.Log.Entries[1].Timings;
            Assert.That(timings.Blocked, Is.Null);
            Assert.That(timings.Dns, Is.Null);
            Assert.That(timings.Connect, Is.Null);
            Assert.That(timings.Ssl, Is.Null);
        }

        [Test]
        public void Deserialize_HarContent_Cache()
        {
            var cache = m_actual.Log.Entries[0].Cache;

            Assert.That(new DateTime(2014, 9, 25, 18, 39, 53), Is.EqualTo(cache.BeforeRequest.Expires));
            Assert.That(new DateTime(2014, 9, 24, 17, 39, 53), Is.EqualTo(cache.BeforeRequest.LastAccess));
            Assert.That("test1", Is.EqualTo(cache.BeforeRequest.ETag));
            Assert.That(1, Is.EqualTo(cache.BeforeRequest.HitCount));
            Assert.That(cache.BeforeRequest, Is.Not.Null);

            Assert.That(cache.AfterRequest, Is.Not.Null);
            Assert.That(new DateTime(2014, 9, 26, 19, 39, 53), Is.EqualTo(cache.AfterRequest.Expires));
            Assert.That(new DateTime(2014, 9, 25, 18, 39, 53), Is.EqualTo(cache.AfterRequest.LastAccess));
            Assert.That("test2", Is.EqualTo(cache.AfterRequest.ETag));
            Assert.That(2, Is.EqualTo(cache.AfterRequest.HitCount));
        }

        [Test]
        public void Deserialize_HarContent_Request()
        {
            var request = m_actual.Log.Entries[0].Request;
            Assert.That("GET", Is.EqualTo(request.Method));
            Assert.That("https://www.google.com/", Is.EqualTo(request.Url.ToString()));
            Assert.That("HTTP/1.1", Is.EqualTo(request.HttpVersion));
            Assert.That(0, Is.EqualTo(request.BodySize));
            Assert.That(1542, Is.EqualTo(request.HeadersSize));
        }

        [Test]
        public void Deserialize_HarContent_Cookies()
        {
            var cookies = m_actual.Log.Entries[0].Request.Cookies;
            Assert.That(9, Is.EqualTo(cookies.Count));

            var cookie = cookies[0];
            Assert.That("PREF", Is.EqualTo(cookie.Name));
            Assert.That("ID=ac8f0e1628ac8f71:U=c1b66ec369dcc09f:FF=0:LD=pt-BR:TM=1409229977:LM=1409230059:GM=1:S=GfV8WG1HURi4SYOq", Is.EqualTo(cookie.Value));
            Assert.That(cookie.Expires.HasValue, Is.False);
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);

            cookie = cookies[1];
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);
        }

        [Test]
        public void Deserialize_HarContent_Headers()
        {
            var headers = m_actual.Log.Entries[0].Request.Headers;
            Assert.That(8, Is.EqualTo(headers.Count));

            var header = headers[0];
            Assert.That("Accept-Encoding", Is.EqualTo(header.Name));
            Assert.That("gzip,deflate,sdch", Is.EqualTo(header.Value));
        }

        [Test]
        public void Deserialize_HarContent_PostData()
        {
            var postData = m_actual.Log.Entries[0].Request.PostData;
            Assert.That(postData, Is.Null);

            postData = m_actual.Log.Entries[25].Request.PostData;
            Assert.That(postData, Is.Not.Null);
            Assert.That("text/ping", Is.EqualTo(postData.MimeType));
            Assert.That("PING", Is.EqualTo(postData.Text));
            Assert.That("PING", Is.EqualTo(postData.Text));
            Assert.That(1, Is.EqualTo(postData.Params.Count));
            Assert.That("test.txt", Is.EqualTo(postData.Params[0].FileName));
            Assert.That("plain/text", Is.EqualTo(postData.Params[0].ContentType));
        }

        [Test]
        public void Deserialize_HarContent_QueryString()
        {
            var queryString = m_actual.Log.Entries[1].Request.QueryString;
            Assert.That(2, Is.EqualTo(queryString.Count));

            var parameter = queryString[0];
            Assert.That("gfe_rd", Is.EqualTo(parameter.Name));
            Assert.That("cr", Is.EqualTo(parameter.Value));
        }

        [Test]
        public void Deserialize_HarContent_Response()
        {
            var response = m_actual.Log.Entries[0].Response;
            Assert.That(302, Is.EqualTo(response.Status));
            Assert.That("Found", Is.EqualTo(response.StatusText));
            Assert.That("HTTP/1.1", Is.EqualTo(response.HttpVersion));
            Assert.That("https://www.google.com/unit/test", Is.EqualTo(response.RedirectUrl.ToString()));
            Assert.That(273, Is.EqualTo(response.HeadersSize));
            Assert.That(0, Is.EqualTo(response.BodySize));

            response = m_actual.Log.Entries[1].Response;
            Assert.That("https://www.google.com.br/?gfe_rd=cr&ei=-Q8jVNr2BteqhQTf0oH4Bw", Is.EqualTo(response.RedirectUrl.ToString()));
        }

        [Test]
        public void Deserialize_HarContent_Content()
        {
            var content = m_actual.Log.Entries[0].Response.Content;
            Assert.That(100, Is.EqualTo(content.Size));
            Assert.That("text/html", Is.EqualTo(content.MimeType));
            Assert.That(10, Is.EqualTo(content.Compression));
            Assert.That("UTF-8", Is.EqualTo(content.Encoding));
            Assert.That("test content", Is.EqualTo(content.Text));

            content = m_actual.Log.Entries[1].Response.Content;
            Assert.That(content.Compression, Is.Null);
        }
    }
}