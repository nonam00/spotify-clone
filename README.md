## Description

As a beginner developer, I'm excited to present my Spotify clone web application, a simple yet functional platform for streaming music online. Built using Next.js and .NET Core, this application offers a user-friendly interface and essential features for music lovers, including the ability to upload and share their own content.

#### Key Features:

- **Basic Search and Filtering**: Find specific songs, and user-uploaded content using our basic search and filtering options.
- **User Uploads**: Upload your own music tracks and share them with the community.

---

## Getting Started

#### First, run WebAPI for the backend:

User secret json file is required to run. *(contact me to get it)*.

Replace my user secret id with yours in *backend/WebAPI/WebAPI.csproj*.

Create selftrusted https sertificate:

```bash
dotnet dev-certs https --trust
```

Then run the API:

```bash
dotnet run --launch-profile https
```

#### Second, run the development server for the frontend:

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

