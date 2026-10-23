export type CareCaseStatus =
  | "AwaitingResult"
  | "AwaitingReview"
  | "Reviewed"
  | "AwaitingPatientContact"
  | "PatientContacted"
  | "FollowUpRequired"
  | "FollowUpInProgress"
  | "Resolved";

export type ResultPriority =
  | "Normal"
  | "Abnormal"
  | "Urgent"
  | "Critical";

export interface CareCaseSummary {
  id: string;
  medicalRecordNumber: string;
  patientName: string;
  testName: string;
  priority: ResultPriority | null;
  status: CareCaseStatus;
  createdAt: string;
}

export interface LoginResponse {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
}