/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'localhost:7025',
        pathname: '**'
      }
    ]
  }
};

export default nextConfig;
