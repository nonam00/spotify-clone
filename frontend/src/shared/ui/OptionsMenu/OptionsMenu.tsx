"use client";

import Image from "next/image";
import * as DropdownMenu from "@radix-ui/react-dropdown-menu";
import { Slot } from "@radix-ui/react-slot";
import { Fragment, ReactNode, useTransition} from "react";
import { HiDotsVertical } from "react-icons/hi";
import { twMerge } from "tailwind-merge";

export type MenuOption = {
  id?: string;
  label: string;
  onSelect: () => void | Promise<void>;
  icon?: ReactNode;
  disabled?: boolean;
  isDestructive?: boolean;
  submenu?: MenuOption[];
  imageUrl?: string;
};

type OptionsMenuProps = {
  options: MenuOption[];
  buttonAriaLabel?: string;
  align?: "start" | "center" | "end";
  side?: "top" | "right" | "bottom" | "left";
  className?: string;
  disabled?: boolean;
  triggerContent?: ReactNode;
  iconSize?: number;
  sideOffset?: number;
  alignOffset?: number;
  avoidCollisions?: boolean;
  // Allow customizing the trigger button
  triggerClassName?: string;
  // For controlled usage
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
  onOpen?: () => void;
};

const OptionsMenu = ({
  options,
  buttonAriaLabel = "Open options menu",
  align = "end",
  side = "bottom",
  className,
  disabled = false,
  triggerContent,
  iconSize = 20,
  sideOffset = 8,
  alignOffset = 0,
  avoidCollisions = true,
  triggerClassName,
  open,
  onOpenChange,
  onOpen,
}: OptionsMenuProps) => {
  const [isPending, startTransition] = useTransition();

  const handleOpenChange = (open: boolean) => {
    if (open && onOpen) {
      onOpen();
    }
    onOpenChange?.(open);
  };

  const handleSelect = async (
    option: MenuOption,
    event: Event,
    hasSubmenu: boolean
  ) => {
    if (option.disabled || hasSubmenu) {
      event.preventDefault();
      return;
    }
    startTransition(async () => {
      await option.onSelect();
    });
  };

  if (options.length === 0) return null;

  const renderMenuItems = (items: MenuOption[], level = 0) => {
    return items.map((option: MenuOption, index) => {
      const hasSubmenu = Boolean(option.submenu?.length);
      const hasImage = Boolean(option.imageUrl);

      return (
        <Fragment key={option.id ?? `${option.label}-${index}-${level}`}>
          {hasSubmenu ? (
            <DropdownMenu.Sub>
              <DropdownMenu.SubTrigger
                disabled={option.disabled || disabled || isPending}
                className={twMerge(
                  "group flex w-full cursor-default select-none items-center gap-2 rounded px-2 py-1.5 text-sm outline-none transition-colors",
                  "data-disabled:pointer-events-none data-disabled:opacity-50",
                  'data-highlighted:bg-neutral-800/70 data-highlighted:text-neutral-200',
                  option.isDestructive
                    ? "text-red-400 data-highlighted:bg-red-500/10"
                    : "text-neutral-200"
                )}
              >
                {hasImage ? (
                  <div className="relative w-5 h-5">
                    <Image
                      src={option.imageUrl!}
                      alt=""
                      className="object-cover"
                      unoptimized
                      loading="lazy"
                      fill
                    />
                  </div>
                ) : option.icon ? (
                  <span className="text-base">{option.icon}</span>
                ) : null}
                {option.label}
              </DropdownMenu.SubTrigger>
              <DropdownMenu.Portal>
                <DropdownMenu.SubContent
                  className="z-50 min-w-32 overflow-hidden rounded-md bg-neutral-900/95 backdrop-blur p-1 shadow-lg ring-1 ring-black/30"
                  sideOffset={2}
                  alignOffset={-5}
                  collisionPadding={16}
                >
                  {renderMenuItems(option.submenu!, level + 1)}
                </DropdownMenu.SubContent>
              </DropdownMenu.Portal>
            </DropdownMenu.Sub>
          ) : (
            <DropdownMenu.Item
              disabled={option.disabled || disabled || isPending}
              onSelect={(event) => handleSelect(option, event, false)}
              className={twMerge(
                "group relative flex cursor-default select-none items-center gap-2 rounded px-2 py-1.5 text-sm outline-none transition-colors",
                "data-disabled:pointer-events-none data-disabled:opacity-50",
                "data-highlighted:bg-neutral-800/70 data-highlighted:text-neutral-200",
                option.isDestructive
                  ? "text-red-400 data-highlighted:bg-red-500/10"
                  : "text-neutral-200"
              )}
            >
              {hasImage ? (
                <div className="relative w-5 h-5">
                  <Image
                    src={option.imageUrl!}
                    alt=""
                    className="object-cover"
                    unoptimized
                    loading="lazy"
                    fill
                  />
                </div>
              ) : option.icon ? (
                <span className="text-base">{option.icon}</span>
              ) : null}
              {option.label}
            </DropdownMenu.Item>
          )}
        </Fragment>
      );
    })
  }

  return (
    <DropdownMenu.Root
      open={open}
      onOpenChange={handleOpenChange}
    >
      <DropdownMenu.Trigger asChild>
        <button
          type="button"
          aria-label={buttonAriaLabel}
          disabled={disabled || isPending}
          className={twMerge(
            "inline-flex items-center justify-center p-2 rounded-full hover:bg-neutral-800/60 transition cursor-pointer outline-none",
            "disabled:opacity-50 disabled:cursor-not-allowed",
            triggerClassName
          )}
        >
          <Slot>
            {triggerContent ?? (
              <HiDotsVertical
                size={iconSize}
                className="text-neutral-400 group-focus:text-neutral-200"
              />
            )}
          </Slot>
        </button>
      </DropdownMenu.Trigger>

      <DropdownMenu.Portal>
        <DropdownMenu.Content
          className={twMerge(
            "z-50 min-w-40 overflow-hidden rounded-md bg-neutral-900/95 p-1 shadow-lg ring-1 ring-black/30",
            "data-[state=open]:animate-in data-[state=closed]:animate-out",
            "data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0",
            "data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95",
            "data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2",
            "data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2",
            className
          )}
          side={side}
          align={align}
          sideOffset={sideOffset}
          alignOffset={alignOffset}
          collisionPadding={16}
          avoidCollisions={avoidCollisions}
          loop
        >
          {renderMenuItems(options)}
          <DropdownMenu.Arrow className="fill-neutral-800/95" />
        </DropdownMenu.Content>
      </DropdownMenu.Portal>
    </DropdownMenu.Root>
  );
};

export default OptionsMenu;