function init() {
    affixTopBar();
    particleBackground();
    zoomableImages();
};

function zoomableImages() {
    var images = document.querySelectorAll('.PostContents > p > img');

    for (var i = 0; i < images.length; i++) {
        var img = images[i];
        img.addEventListener('click', toggleZoom);
    }
}

function toggleZoom(image) {
    var img = image.target
    if (img.hasAttribute('data-original')) {
        img.style.width = '100%';
        img.removeAttribute('data-original')
    } else {
        img.style.width = 'inherit';
        img.setAttribute('data-original', true)
    }
}

function affixTopBar() {
    var topBar = document.getElementsByClassName('Main-header')[0];

    var headerHeight = topBar.style.height;

    window.addEventListener('scroll', function () {
        var currentTop = window.scrollY;
        if ((currentTop < this.previousTop || 0) && !(currentTop > 0 && topBar.classList.contains('is-fixed'))) {
            topBar.classList.remove('is-fixed');
        } else {
            if (currentTop > headerHeight && !topBar.classList.contains('is-fixed')) {
                topBar.classList.add('is-fixed');
            }
        }
        this.previousTop = currentTop;
    }, false);
}

function particleBackground() {
    if (window.particlesJS) {
        window.particlesJS('headerBackground');
    } else {
        setTimeout(particleBackground);
    }
}

if (
    document.readyState === 'complete' ||
    (document.readyState !== 'loading' && !document.documentElement.doScroll)
) {
    init();
} else {
    document.addEventListener('DOMContentLoaded', init);
}
