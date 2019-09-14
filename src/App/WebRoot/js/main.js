function init() {
    animateTopBar();
    particleBackground();
    resizeTitleCanvas();

    window.addEventListener('resize', resizeTitleCanvas);

};

function resizeTitleCanvas() {
    const headerTitle = document.querySelector('.Main-titleContainer');
    const headerBackground = document.getElementById('headerBackground');

    const titleHeight = headerTitle.offsetHeight;
    if(titleHeight < 300) { return; }
    headerBackground.style.height = titleHeight + 'px';
};

function animateTopBar() {
    const topBar = document.getElementsByClassName('Main-header')[0];

    const headerHeight = topBar.style.height;

    window.addEventListener('scroll', function() {
        const currentTop = window.scrollY;
        if (currentTop < this.previousTop || 0) {
            if (currentTop > 0 && topBar.classList.contains('is-fixed')) {
                topBar.classList.add('is-visible');
            } else {
                topBar.classList.remove('is-visible');
                topBar.classList.remove('is-fixed');
            }
        } else {
            topBar.classList.remove('is-visible');
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
