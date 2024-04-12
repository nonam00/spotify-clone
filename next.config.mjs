/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'pfrxorzfwpmiywfqnfjb.supabase.co',
        pathname: '**'
      }
    ]
  }
};

export default nextConfig;
