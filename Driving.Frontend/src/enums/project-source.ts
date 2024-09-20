export function getProjectSourceName(index?: number) {
  return projectSource.find((x) => x.value === index)?.name ?? "N/A";
}

export const projectSource = [
  { name: "Hays", value: 0 },
  { name: "Freelance.de", value: 1 },
  { name: "FreelancerMap", value: 2 },
];
