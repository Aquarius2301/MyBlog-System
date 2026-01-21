function convertToMB(sizeInBytes: number): string {
  return (sizeInBytes / 1024 / 1024).toFixed(2) + " MB";
}

export { convertToMB };
