import type {
  CareCaseSummary,
  LoginResponse,
} from "./types";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "";

export async function login(
  email: string,
  password: string,
): Promise<LoginResponse> {
  const response = await fetch(
    `${API_BASE_URL}/auth/login`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email,
        password,
      }),
    },
  );

  if (!response.ok) {
    throw new Error("Invalid email or password.");
  }

  return response.json();
}

export async function getCareCases(
  token: string,
): Promise<CareCaseSummary[]> {
  const response = await fetch(
    `${API_BASE_URL}/api/care-cases`,
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    },
  );

  if (response.status === 401) {
    throw new Error("UNAUTHORIZED");
  }

  if (!response.ok) {
    throw new Error(
      "Unable to load CareCases.",
    );
  }

  return response.json();
}