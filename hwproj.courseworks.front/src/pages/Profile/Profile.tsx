import React from "react";
import { Redirect, RouteComponentProps } from "react-router-dom";

import MainWindow from "../../Components/MainWindow/MainWindow";
import { IUser } from "types";
import Menu from "./Menu/Menu";
import TopBar from "./TopBar/TopBar";
import {apiDev} from "../../api-dev";

interface ProfileProps extends RouteComponentProps {
  user: IUser;
  token: string;
  logout: () => void;
}

export default function Profile({ user, token, logout }: ProfileProps) {
  const [courseworks, setCourseWorks] = React.useState([]);

  const fetchCourseWorks = React.useCallback(async () => {
      const {data} = await apiDev.get('courseworks');
      setCourseWorks(data);
  }, []);

  React.useEffect(() => {
      if (process.env.NODE_ENV === 'development') {
          fetchCourseWorks();
      }
  }, [fetchCourseWorks])

  if (!token) {
    return <Redirect to="/login" />;
  }

  return (
    <>
      <div className="topBar">
        <TopBar user={user} logout={logout} fetchCourseWorks={fetchCourseWorks} />
      </div>
      <Menu
        role={user.role}
        page=""
        changePage={() => {}}
        isCritic={user.isCritic}
      />
      <div style={{ position: "relative", left: "17vw", top: "16vh" }}>
        <MainWindow
          courseworks={courseworks}
          newChangePage={() => {}}
          token={token}
          role={user.role}
          page=""
          changePage={() => {}}
          handleCritic={() => {}}
          isCritic={user.isCritic}
          userId={user.userId}
        />
      </div>
    </>
  );
}
