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
    colors: {
      transparent: "transparent",
      current: "currentColor",
      black: "#000000",
      white: "#ffffff",
      purple: "#dd7dff",
      "purple-dark": "#620085",
      green: "#00ff75",
      yellow: "#fff503",
      red: "#ff5d5d",
      orange: "#ffb443",
      blue: "#38dbff",
    },
  },
  plugins: [],
};
