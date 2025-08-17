import { redirect } from "next/navigation";
import Image from "next/image";
import { IoMdPerson, IoMdMail } from "react-icons/io";

import getUserInfo from "@/actions/user/getUserInfo";
import Header from "@/components/Header";
import ChangeAvatarForm from "@/app/account/components/ChangeAvatarForm";
import ChangePasswordForm from "@/app/account/components/ChangePasswordForm";
import {CLIENT_API_URL} from "@/helpers/api";

const AccountPage = async () => {
  const userDetails = await getUserInfo();

  if (!userDetails) {
    redirect("/");
  }

  return (
    <div className="h-full w-full bg-neutral-900 rounded-lg overflow-y-auto">
      <Header>
        <div className="mb-2 flex flex-col gap-y-6">
          <h1 className="text-white text-3xl font-bold">
            Account Settings
          </h1>
        </div>
      </Header>
      <div className="h-full py-6">
        <div className="max-w-4xl mx-auto px-4">
          <p className="text-neutral-400 mb-1">
            Manage your account information and preferences
          </p>

          {/* User Info Card */}
          <div className="bg-neutral-800/50 rounded-lg p-6 mb-8">
            <div className="flex items-center space-x-4">
              {userDetails.avatarPath ? (
                <Image
                  src={`${CLIENT_API_URL}/files/image/${userDetails.avatarPath}`}
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
              <ChangeAvatarForm userDetails={userDetails} />
            </div>
            <div>
              <ChangePasswordForm />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AccountPage;
