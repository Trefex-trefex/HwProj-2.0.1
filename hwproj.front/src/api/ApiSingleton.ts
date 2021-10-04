import {
  AccountApi,
  NotificationsApi,
  CoursesApi,
  SolutionsApi,
  HomeworksApi,
  TasksApi,
  StatisticsApi,
  CourseGroupsApi
} from ".";
import { Configuration } from './configuration';
import AuthService from "../services/AuthService";


class Api {
  auth = new AuthService()
  readonly accountApi: AccountApi;
  readonly coursesApi: CoursesApi;
  readonly solutionsApi: SolutionsApi;
  readonly notificationsApi: NotificationsApi;
  readonly homeworksApi: HomeworksApi;
  readonly tasksApi: TasksApi;
  readonly statisticsApi: StatisticsApi;
  readonly authService: AuthService;
  readonly courseGroupsApi: CourseGroupsApi;

  constructor(
    accountApi: AccountApi,
    coursesApi: CoursesApi,
    solutionsApi: SolutionsApi,
    notificationsApi: NotificationsApi,
    homeworksApi: HomeworksApi,
    tasksApi: TasksApi,
    statisticsApi: StatisticsApi,
    authService: AuthService,
    courseGroupsApi: CourseGroupsApi,
  ) {
    this.accountApi = accountApi;
    this.coursesApi = coursesApi;
    this.solutionsApi = solutionsApi;
    this.notificationsApi = notificationsApi;
    this.homeworksApi = homeworksApi;
    this.tasksApi = tasksApi;
    this.statisticsApi = statisticsApi;
    this.authService = authService;
    this.courseGroupsApi = courseGroupsApi;
  }
}

const currentToken = (new AuthService()).getToken()
const token = typeof currentToken === "string"
  ? currentToken
  : undefined
const basePath = "http://localhost:5000";

const authService = new AuthService();
let ApiSingleton: Api;
ApiSingleton = new Api(
  new AccountApi({ basePath: basePath, apiKey: () => "Bearer " + authService.getToken()! }),
  new CoursesApi({ basePath: basePath, apiKey: () => "Bearer " + authService.getToken()! }),
  new SolutionsApi({ basePath: basePath, apiKey: () => "Bearer " + authService.getToken()! }),
  new NotificationsApi({basePath: basePath, apiKey: () => "Bearer " + authService.getToken()! }),
  new HomeworksApi({basePath: basePath, apiKey: () => "Bearer " + authService.getToken()! }),
  new TasksApi({ basePath: basePath, apiKey: () => "Bearer " + authService.getToken()! }),
  new StatisticsApi({ basePath: basePath, apiKey: () => "Bearer " + authService.getToken()! }),
  authService,
    new CourseGroupsApi({ basePath: basePath, apiKey: () => "Bearer " + authService.getToken()! }),
);
export default ApiSingleton;
