"use client";

import {memo} from "react";
import * as RadixSlider from "@radix-ui/react-slider";
import {twMerge} from "tailwind-merge";

type CustomSliderProps = Omit<RadixSlider.SliderProps, "value" | "onDragStart"> & {
  value: number,
  isLoading?: boolean,
  onDragStart?: () => void,
};

const Slider = memo(function Slider({
  value = 0,
  onValueChange,
  onValueCommit,
  onDragStart,
  isLoading,
  disabled,
}: CustomSliderProps) {
  return (
    <RadixSlider.Root
      className="relative group flex items-center select-none touch-none w-full h-7 cursor-pointer"
      defaultValue={[1]}
      value={[value]}
      onValueChange={onValueChange}
      onValueCommit={onValueCommit}
      onPointerDown={onDragStart}
      max={1}
      step={0.01}
      disabled={disabled}
    >
      <RadixSlider.Track
        className={twMerge(
          "bg-neutral-600 relative grow rounded-full h-[3.1px]",
          isLoading ? "animate-pulse" : ""
        )}
      >
        <RadixSlider.Range className="absolute bg-white group-hover:bg-green-500 rounded-full h-full" />
      </RadixSlider.Track>
      <RadixSlider.Thumb
        className={twMerge(
          "block w-3 h-3 bg-white rounded-full shadow-sm opacity-0 " +
          "group-hover:opacity-100 hover:shadow-md focus:outline-none focus:shadow-lg transition-all",
          isLoading ? "bg-neutral-400" : ""
        )}
      />
    </RadixSlider.Root>
  );
});

export default Slider;