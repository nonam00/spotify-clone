/** @type {import('next').NextConfig} */
module.exports = {
  output: 'standalone',
  images: {
    remotePatterns: [
      {
        protocol: 'http',
        hostname: 'localhost:5000',
        pathname: '**'
      }
    ]
  }
}
