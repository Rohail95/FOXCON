// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Handles the plus/minus icon for the category collapse in the off-canvas menu
var categoryCollapse = document.getElementById('categoryCollapse');
if (categoryCollapse) {
    var toggleButton = document.querySelector('a[href="#categoryCollapse"]');
    var icon = toggleButton.querySelector('.fas');

    categoryCollapse.addEventListener('show.bs.collapse', function () {
        icon.classList.remove('fa-plus');
        icon.classList.add('fa-minus');
    });

    categoryCollapse.addEventListener('hide.bs.collapse', function () {
        icon.classList.remove('fa-minus');
        icon.classList.add('fa-plus');
    });
}