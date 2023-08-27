/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./**/*.{html,razor,cshtml,css,scss,cs}"],
  theme: {
    extend: {},
  },
  plugins: [require('@tailwindcss/typography'),require("daisyui")],
}

