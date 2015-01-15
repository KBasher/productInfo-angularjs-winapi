using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CommonwealthGames.DataLayer;
using Newtonsoft.Json;

namespace CommonwealthGames.Controllers
{
    public class ProductController : ApiController
    {
        private ProductInfoEntities db = new ProductInfoEntities();
        public HttpResponseMessage GetProducts()
        {
            var collection = db.Products.Select(x => new
            {
                id = x.id,
                name = x.name,
                imageUrl = x.imageUrl
            });
            var yourJson = new JavaScriptSerializer().Serialize(collection);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
            return response;
        }
        public HttpResponseMessage GetProduct(int id)
        {
            var productModel= db.Products.Include("ProductDetail").Where(p => p.id == id).FirstOrDefault();
            if (productModel.ProductDetail != null)
            {
                var product = db.Products.Include("ProductDetail").Where(p => p.id == id).Select(x => new
                {
                    id = x.id,
                    name = x.name,
                    imageUrl = x.imageUrl,
                    ProductDetail = new { id = x.ProductDetail.id, number = x.ProductDetail.number, price = x.ProductDetail.price, description = x.ProductDetail.description, companyName = x.ProductDetail.companyName }
                }).FirstOrDefault();
                var json = new JavaScriptSerializer().Serialize(product);
                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                var product = db.Products.Include("ProductDetail").Where(p => p.id == id).Select(x => new
                {
                    id = x.id,
                    name = x.name,
                    imageUrl = x.imageUrl,
                    ProductDetail = new { id = 0, number = 0, price = 0, description = "", companyName = "" }
                }).FirstOrDefault();
                var json = new JavaScriptSerializer().Serialize(product);
                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");
                return response;
            }
        }
        public async Task<HttpResponseMessage> Put()
        {
            string imageUrl = "";
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/uploads");
                CustomMultipartFormDataStreamProvider streamProvider = new CustomMultipartFormDataStreamProvider(uploadPath);

                await Request.Content.ReadAsMultipartAsync(streamProvider);

                List<string> messages = new List<string>();
                foreach (var file in streamProvider.FileData)
                {
                    FileInfo fi = new FileInfo(file.LocalFileName);
                    imageUrl = "/uploads/" + fi.Name;
                }

                foreach (var val in streamProvider.FormData.GetValues("product"))
                {
                    Product product = JsonConvert.DeserializeObject<Product>(val);
                    product.imageUrl = imageUrl;

                    if (ModelState.IsValid)
                    {
                        Product productEntity = db.Products.Where(p => p.id == product.id).FirstOrDefault();
                        productEntity.name = product.name;
                        productEntity.imageUrl = product.imageUrl;
                        productEntity.ProductDetail.number = product.ProductDetail.number;
                        productEntity.ProductDetail.price = product.ProductDetail.price;
                        productEntity.ProductDetail.description = product.ProductDetail.description;
                        productEntity.ProductDetail.companyName = product.ProductDetail.companyName;
                        db.Entry(productEntity).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                        }
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }

        }
        public async Task<HttpResponseMessage> Post()
        {
            string imageUrl = "";
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/uploads");
                CustomMultipartFormDataStreamProvider streamProvider = new CustomMultipartFormDataStreamProvider(uploadPath);

                await Request.Content.ReadAsMultipartAsync(streamProvider);

                List<string> messages = new List<string>();
                foreach (var file in streamProvider.FileData)
                {
                    FileInfo fi = new FileInfo(file.LocalFileName);
                    imageUrl = "/uploads/" + fi.Name;
                }

                foreach (var val in streamProvider.FormData.GetValues("product"))
                {
                    Product product = JsonConvert.DeserializeObject<Product>(val);
                    product.imageUrl = imageUrl;

                    if (ModelState.IsValid)
                    {
                        db.Products.Add(product);
                        db.ProductDetails.Add(product.ProductDetail);
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }
        
        public HttpResponseMessage DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            db.Products.Remove(product);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, product);
        }
    }
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
            return name.Replace("\"", string.Empty); 
        }
    }
    
    public class RecipeInformation
    {
        public string name { get; set; }
    }
}
