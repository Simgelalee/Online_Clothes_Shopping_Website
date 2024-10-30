// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.onload = function () {
    var myDivs = document.querySelectorAll('.swiper-container');
    if (myDivs.length > 0) {
        myDivs.forEach(function (myDiv) {
            myDiv.style.height = '70vh';
        });
    }
};
  

// Sepete ürün eklemek için AJAX kodu
document.getElementById('addToCartForm').addEventListener('submit', function (e) {
    e.preventDefault(); // Formun normal submit işlemini engelle
    // Form verilerini al
    var formData = new FormData(this);
    // Form verilerini AJAX ile gönder
    fetch('/Home/Details', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            console.log(response)
            response.json()
        })
        .then(data => {
            // İşlem başarılıysa 
            console.log(data)
            console.log('Ürün sepete eklendi');
            // Başka bir sayfaya yönlendirme yapabilirsiniz
            // window.location.href = '/AnaSayfa';
        })
        .catch(error => {
            // İşlem başarısızsa
            console.error('Ürün sepete eklenemedi');
            console.log(error)
        });
});
