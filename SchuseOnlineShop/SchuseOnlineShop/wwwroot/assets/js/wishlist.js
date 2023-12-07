// SIDEBAR
let iconx = document.querySelector(".iconx");
let icon = document.querySelector(".fa-bars");
let overlay = document.querySelector(".overlay");

icon.onclick = function () {
  var sidebar = document.querySelector(".content-area");
  sidebar.style.width = "80%";  
  sidebar.style.visibility = "visible";
  sidebar.style.opacity = "1";
};

iconx.onclick = function () {
  var sidebar = document.querySelector(".content-area");
  sidebar.style.width = "0%";
  sidebar.style.visibility = "hidden";
  sidebar.style.opacity = "0";
};



$(document).ready(function () {
  //Bir-başa headerə qaytarn icon
  // scroll to top
  $(window).on('scroll', function () {
    if ($(this).scrollTop() > 600) {
      $('.scroll-top').removeClass('not-visible');
    } else {
      $('.scroll-top').addClass('not-visible');
    }
  });
  $('.scroll-top').on('click', function (event) {
    $('html,body').animate({
      scrollTop: 0
    }, 1000);
  });


});

//Sticky--menu 

$(function(){
  "use strict";

  // Sticky menu 
  var $window = $(window);
  $window.on('scroll', function () {
    var scroll = window.scrollY;
    if (scroll < 200) {
      $(".down-navbar").removeClass("stick-nav");
    } else {
      $(".down-navbar").addClass("stick-nav");
    }
  });





})


//-------wishlist start-------------------


// let tableBody = document.querySelector("tbody")

// let wishlists = JSON.parse(localStorage.getItem("wishlist"))

// if (wishlists != null) {
//     for (const product of wishlists) {
//         tableBody.innerHTML += `<tr data-id = "${product.id}">
//         <td class="td-img">
//         <div class="prdct-img">
//         <img src="${product.img}" alt=""></td>
//         <td class="td-name">${product.name}</td>
//         <td class="td-price">${product.price}.00</td>
//         <td><i class="fa-solid fa-xmark delete"></i></td>
//         </tr>`
//     }

//     // getBasketCount(products)


// }
// else {
//     document.querySelector("table").classList.add("d-none")
//     document.querySelector(".empty-contunie").classList.remove("d-none")
// }


// let dltn = document.querySelector("table .btn")

// dltn.addEventListener("click", function () {
//     localStorage.removeItem("wishlist");
//     document.querySelector("table").classList.add("d-none")
//     document.querySelector(".empty-contunie").classList.remove("d-none")
// })



// function deleteWishlist(id) {
//     let wshlstProduct = wishlists.filter(m => m.id != id);
//     wishlists = wshlstProduct
//     localStorage.setItem("wishlist", JSON.stringify(wishlists));
// }

// let deleteIcons = document.querySelectorAll(".delete");

// deleteIcons.forEach(icon => {
//     icon.addEventListener("click", function () {
//         let id = parseInt(this.parentNode.parentNode.getAttribute("data-id"))
//         deleteWishlist(id)
//         this.parentNode.parentNode.remove();
//         if (wishlists.length == 0) {
//             localStorage.removeItem("wishlist");
//             document.querySelector("table").classList.add("d-none")
//             document.querySelector(".empty-contunie").classList.remove("d-none")
//         }


//     })
// });

//-------wishlist end-------------------




$(function () {
    //-----------Delete-Wishlist

    //$(document).on("click", ".delete-product", function () {

    //    let id = $(this).parent().parent().attr("data-id");
    //    console.log(id)
    //    let prod = $(this).parent().parent();
    //    let tbody = $(".tbody").children();
    //    let data = { id: id };

    //    $.ajax({
    //        type: "Post",
    //        url: `Wishlist/DeleteDataFromWishlist`,
    //        data: data,
    //        success: function () {
    //            if ($(tbody).length == 1) {
    //                $(".product-table").addClass("d-none");
    //                //$(".footer-alert").removeClass("d-none")
    //            }
    //            $(prod).remove();
    //        }
    //    })
    //    return false;
    //})




    $(document).on("click", ".delete-product", function () {

        var id = $(this).data('id')
        var basketCount = $('.count-whislist')
        var basketCurrentCount = $('.count-whislist').html()
        let tbody = $(".tbody").children();
        var quantity = $(this).data('quantity')
        var sum = basketCurrentCount - quantity

        $.ajax({
            method: 'POST',
            url: "/wishlist/delete",
            data: {
                id: id
            },
            success: function (res) {
                if ($(tbody).length == 1) {
                    $(".product-table").addClass("d-none");
                    //$(".footer-alert").removeClass("d-none")
                }


                $(`.wishlist-product[data-id=${id}]`).remove();
                basketCount.html("")
                basketCount.append(sum)



            }
        })
    })

})