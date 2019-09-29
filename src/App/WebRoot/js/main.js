function init() {
    affixTopBar();
    particleBackground();
    resizeTitleCanvas();
    resizeHeaderNav();

    window.addEventListener('resize', resizeTitleCanvas);

    setTimeout(function () {
        window.scrollTo(0, window.scrollY);
    }, 100);
};

function resizeHeaderNav() {
    var headerBar = document.querySelector('.Main-header');

    var viewWidth = document.documentElement.clientWidth - 8;
    headerBar.style.width = viewWidth + 'px';
};

function resizeTitleCanvas() {
    var headerTitle = document.querySelector('.Main-titleContainer');
    var headerBackground = document.getElementById('headerBackground');

    var titleHeight = headerTitle.offsetHeight;
    if (titleHeight < 300) { return; }
    headerBackground.style.height = titleHeight + 'px';
};

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
        window.particlesJS.load('headerBackground', '/assets/particles.json');
        resizeTitleCanvas();
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
