﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Carrusel de fondo solo en .hero-bg-carousel (a la derecha)
// Este código ya no debe usarse porque el fondo ahora es dinámico desde Razor y el admin
// (function() {
//   const images = [
//     '/images/carru1.jpg',
//     '/images/carru2.jpe',
//     '/images/carru3.jpg'
//   ];
//   const carousel = document.querySelector('.hero-bg-carousel');
//   if (!carousel) return;
//   images.forEach((src, idx) => {
//     const div = document.createElement('div');
//     div.className = 'bg-image' + (idx === 0 ? ' active' : '');
//     div.style.backgroundImage = `url('${src}')`;
//     carousel.appendChild(div);
//   });
//   const bgDivs = carousel.querySelectorAll('.bg-image');
//   let current = 0;
//   setInterval(() => {
//     const prev = current;
//     current = (current + 1) % images.length;
//     bgDivs[prev].classList.add('disintegrate');
//     bgDivs[current].classList.add('active');
//     setTimeout(() => {
//       bgDivs[prev].classList.remove('active', 'disintegrate');
//     }, 1000);
//   }, 4000);
// })();

window.updateCartBadge = function(count) {
  sessionStorage.setItem('cartCount', count);
  var badge = document.getElementById('cart-count-badge');
  if (badge) {
    badge.textContent = count;
    badge.style.display = count > 0 ? '' : 'none';
  }
  // Persistir en localStorage para la vista de carrito
  localStorage.setItem('cartCount', count);
}
// Sincronizar badge al cargar la página
(function() {
  var badge = document.getElementById('cart-count-badge');
  // Leer la cantidad real de productos del carrito (sumar cantidades)
  var carrito = JSON.parse(localStorage.getItem('carrito')) || [];
  var count = carrito.reduce((acc, p) => acc + (p.cantidad || 1), 0);
  if (badge) {
    badge.textContent = count;
    badge.style.display = count > 0 ? '' : 'none';
  }
  // También sincroniza en sessionStorage para compatibilidad
  sessionStorage.setItem('cartCount', count);
})();
