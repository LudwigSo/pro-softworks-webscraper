import { ProjectApi, TagApi, Configuration, KeywordApi } from "@/api/";

const config = new Configuration({ basePath: "http://localhost:8080" });

export const tagApi = new TagApi(new Configuration(config));
export const projectApi = new ProjectApi(new Configuration(config));
export const keyWordApi = new KeywordApi(new Configuration(config));
