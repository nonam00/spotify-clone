import type { Metadata } from "next";
import { Figtree } from "next/font/google";
import "./globals.css";

import {getUserPlaylists} from "@/entities/playlist/api";
import { Player } from "@/widgets/player";
import { Sidebar } from "@/_app/ui";
import { AuthProvider, ModalProvider, ToasterProvider }from "@/_app/providers";

const font = Figtree({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Spotify Clone",
  description: "Listen to music",
  icons: {
    icon: "/icon.svg",
  }
};

export const revalidate = 0;

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const playlists = await getUserPlaylists();
  return (
    <html lang="en">
      <body className={font.className}>
        <ToasterProvider />
        <AuthProvider>
          <ModalProvider />
          <Sidebar playlists={playlists}>
            {children}
          </Sidebar>
          <Player />
        </AuthProvider>
      </body>
    </html>
  );
}
