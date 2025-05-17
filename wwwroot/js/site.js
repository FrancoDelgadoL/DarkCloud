// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Carrusel de fondo solo en .hero-bg-carousel (a la derecha)
(function() {
  const images = [
    '/images/carru1.jpg',
    '/images/carru2.jpe',
    '/images/carru3.jpg'
  ];
  const carousel = document.querySelector('.hero-bg-carousel');
  if (!carousel) return;
  images.forEach((src, idx) => {
    const div = document.createElement('div');
    div.className = 'bg-image' + (idx === 0 ? ' active' : '');
    div.style.backgroundImage = `url('${src}')`;
    carousel.appendChild(div);
  });
  const bgDivs = carousel.querySelectorAll('.bg-image');
  let current = 0;
  setInterval(() => {
    const prev = current;
    current = (current + 1) % images.length;
    bgDivs[prev].classList.add('disintegrate');
    bgDivs[current].classList.add('active');
    setTimeout(() => {
      bgDivs[prev].classList.remove('active', 'disintegrate');
    }, 1000);
  }, 4000);
})();
