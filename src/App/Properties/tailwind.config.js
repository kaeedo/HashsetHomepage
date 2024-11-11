/** @type {import('tailwindcss').Config} */
// https://paletton.com/#uid=7380M0kAMsLmXBktD-oEtnhKhhT
module.exports = {
    content: ["../**/*.fs", "../WebRoot/js/**/*.js"],
    theme: {
        extend: {
            spacing: {
                112: "28rem",
                128: "32rem",
                136: "34rem",
                204: "51rem"
            },
        },

        // https://mdigi.tools/lighten-color/#000000
        colors: {
            transparent: "transparent",
            current: "currentColor",
            black: "#000000",
            gray: "#a6a6a6",
            white: "#ffffff",
            purple: "#cd76ea",
            "purple-dark": "#54106b", // 65% darker
            green: "#00ff75",
            yellow: "#fff500",
            red: "#ff5e5e",
            orange: "#ffb443",
            "blue-light": "#baf2ff", // 65% lighter
            blue: "#39dbff",
            "blue-dark": "#00596d", // 65% darker
        },
    },
    plugins: [require("@tailwindcss/typography")],
};
