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
            },
        },
        // https://mdigi.tools/lighten-color/#000000
        colors: {
            transparent: "transparent",
            current: "currentColor",
            black: "#000000",
            gray: "#a6a6a6",
            white: "#ffffff",
            purple: "#dd7dff",
            "purple-dark": "#620085", // 65% darker
            green: "#00ff75",
            yellow: "#fff503",
            red: "#ff5d5d",
            orange: "#ffb443",
            "blue-light": "#b9f2ff", // 65% lighter
            blue: "#38dbff",
            "blue-dark": "#00596d", // 65% darker
        },
    },
    plugins: [require("@tailwindcss/typography")],
};
