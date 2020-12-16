﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Models;
using ProductCatalog.Data;
using ProductCatalog.ViewModels.ProductViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ProductCatalog.ViewModels;

namespace ProductCatalog.Controllers
{
    public class ProductController : Controller
    {
        private readonly StoreDataContext _context;

        public ProductController(StoreDataContext context)
        {
            _context = context;
        }

        [Route("v1/products")]
        [HttpGet]
        public IEnumerable<ListProductViewModel> Get()
        {
            return _context.Products
                .Include(x => x.Category)
                .Select(x => new ListProductViewModel(x.Id, x.Title, x.Price, x.Category.Id, x.Category.Title))
                .AsNoTracking()
                .ToList();           
        }

        [Route("v1/products/{id}")]
        [HttpGet]
        public Product Get(int id)
        {
            return _context.Products.AsNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        [Route("v1/Products")]
        [HttpPost]
        public ResultViewModel Post([FromBody]EditorProductViewModel model)
        {
            model.Validate();
            if (model.Invalid)
                return new ResultViewModel
                {
                    Success = false,
                    Message = "Não foi possível cadastrar o produto :(",
                    Data = model.Notifications
                };

            var product = new Product();
            product.Title = model.Title;
            product.CategoryId = model.CategoryId;
            product.CreateDate = DateTime.Now;
            product.Description = model.Description;
            product.Image = model.Image;
            product.LastUpdateDate = DateTime.Now;
            product.Price = model.Price;
            product.Quantity = model.Quantity;

            _context.Products.Add(product);
            _context.SaveChanges();

            return new ResultViewModel
            {
                Success = true,
                Message = "O Produto foi cadastrado com sucesso! :)",
                Data = product
            };
        }

        [Route("v1/products")]
        [HttpPut]
        public ResultViewModel Put([FromBody]EditorProductViewModel model)
        {
            model.Validate();
            if (model.Invalid)
                return new ResultViewModel
                {
                    Success = false,
                    Message = "Não foi possível atualizar o produto :(",
                    Data = model.Notifications
                };

            var product = _context.Products.Find(model.Id);
            product.Title = model.Title;
            product.CategoryId = model.CategoryId;            
            product.Description = model.Description;
            product.Image = model.Image;
            product.LastUpdateDate = DateTime.Now;
            product.Price = model.Price;
            product.Quantity = model.Quantity;

            _context.Entry<Product>(product).State = EntityState.Modified;
            _context.SaveChanges();

            return new ResultViewModel
            {
                Success = true,
                Message = "O Produto foi atualizado com sucesso! :)",
                Data = product
            };
        }
    }
}
