import type { Metadata } from "next";
import { Figtree } from "next/font/google";
import "./globals.css";

import Sidebar from "@/components/Sidebar";
import Player from "@/components/Player";

import UserProvider from "@/providers/UserProvider";
import ModalProvider from "@/providers/ModalProvider";
import ToasterProvider from "@/providers/ToasterProvider";

import getUserPlaylists from "@/actions/playlists/getUserPlaylists";

const font = Figtree({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Spotify Clone",
  description: "Listen to music",
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
        <ToasterProvider/>
        <UserProvider>
          <ModalProvider/>
          <Sidebar playlists={playlists}>
            {children}
          </Sidebar>
          <Player />
        </UserProvider>
      </body>
    </html>
  );
}
