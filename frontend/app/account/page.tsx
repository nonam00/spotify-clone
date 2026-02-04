import { redirect } from "next/navigation";
import Image from "next/image";
import { IoMdPerson, IoMdMail } from "react-icons/io";

import { CLIENT_FILES_URL } from "@/shared/config/api";
import {getUserInfoServer} from "@/entities/user";
import { Header } from "@/widgets/header";
import {
  ChangeUserInfoForm,
  ChangeUserPasswordForm,
} from "@/widgets/account-settings";

export default async function AccountPage() {
  const userDetails = await getUserInfoServer();

  if (!userDetails) {
    redirect("/");
  }

  return (
    <div className="h-full w-full bg-neutral-900 rounded-lg overflow-y-auto outline-none">
      <Header>
        <div className="mb-2 flex flex-col gap-y-6">
          <h1 className="text-white text-3xl font-bold">Account Settings</h1>
        </div>
      </Header>
      <div className="h-full py-6">
        <div className="max-w-4xl mx-auto px-4">
          <p className="text-neutral-400 mb-1">Manage your account information and preferences</p>

          {/* User Info Card */}
          <div className="bg-neutral-800/50 rounded-lg p-6 mb-8">
            <div className="flex items-center space-x-4">
              {userDetails.avatarPath ? (
                <Image
                  src={`${CLIENT_FILES_URL}/download-url?type=image&file_id=${userDetails.avatarPath}`}
                  alt="Avatar" 
                  className="w-16 h-16 rounded-full object-cover"
                  loading="lazy"
                  unoptimized
                />
              ) : (
                <div className="w-16 h-16 bg-white rounded-full flex items-center justify-center">
                  <IoMdPerson className="w-8 h-8 text-black" />
                </div>
              )}
              <div>
                <h2 className="text-xl font-semibold text-white">
                  {userDetails.fullName || "User"}
                </h2>
                <p className="text-neutral-400 flex items-center">
                  <IoMdMail className="w-4 h-4 mr-2" />
                  {userDetails.email}
                </p>
              </div>
            </div>
          </div>

          {/* Settings Sections */}
          <div className="flex flex-col gap-8">
            <div>
              <ChangeUserInfoForm userDetails={userDetails} />
            </div>
            <div>
              <ChangeUserPasswordForm />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};