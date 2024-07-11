/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'localhost:7025',
        pathname: '**'
      },
      {
        protocol: 'https',
        hostname: 'spotify-clone-bucket.s3.cloud.ru',
        pathname: '**'
      }
    ]
  }
};

export default nextConfig;
