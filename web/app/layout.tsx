import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { AuthProvider } from "@/context/AuthContext";
import { QueryProvider } from "@/provider/QueryProvider";

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html
      lang="en"
      className={`h-full antialiased`}
    >
      <body className="bg-zinc-800 min-h-full flex flex-col"><QueryProvider>
          <AuthProvider>
            {children}
          </AuthProvider>
        </QueryProvider></body>
    </html>
  );
}
