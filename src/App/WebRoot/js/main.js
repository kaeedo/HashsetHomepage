function init() {
    animateTopBar();
    particleBackground();
};

function animateTopBar() {
    const topBar = document.getElementsByClassName('Main-header')[0];

    const headerHeight = topBar.style.height;

    window.addEventListener('scroll', function() {
        const currentTop = window.scrollY;
        //check if user is scrolling up
        if (currentTop < this.previousTop || 0) {
            console.log('going up')

            //if scrolling up...
            if (currentTop > 0 && topBar.classList.contains('is-fixed')) {
                topBar.classList.add('is-visible');
            } else {
                topBar.classList.remove('is-visible');
                topBar.classList.remove('is-fixed');
            }
        } else {
            //if scrolling down...
            console.log('down')
            topBar.classList.remove('is-visible');
            if (currentTop > headerHeight && !topBar.classList.contains('is-fixed')) {
                topBar.classList.add('is-fixed');
            }
        }
        this.previousTop = currentTop;
    }, false);
}

function particleBackground() {
    if (particlesJS) {
        console.log('checking')
        particlesJS.load('headerBackground', 'assets/particles.json');
    } else {
        setTimeout(particleBackground, 100);
    }

}

if (
    document.readyState === "complete" ||
    (document.readyState !== "loading" && !document.documentElement.doScroll)
) {
    init();
} else {
    document.addEventListener("DOMContentLoaded", init);
}
