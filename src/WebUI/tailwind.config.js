/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./Components/**/*.{razor,cshtml,css,js}", "./Pages/**/*.{razor,cshtml,css,js}", "./Shared/**/*.{razor,cshtml,css,js}","./wwwroot/**/*.{razor,cshtml,css,js}"],
  theme: {
    extend: {},
  },
  plugins: [require('@tailwindcss/typography'),require("daisyui")],
    mode: "jit",
    darkMode: "class",
    daisyui: {          
    }   
};

