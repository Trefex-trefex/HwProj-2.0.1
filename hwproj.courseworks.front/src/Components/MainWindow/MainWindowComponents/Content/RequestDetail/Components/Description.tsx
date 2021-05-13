import React from "react";
import Typography from "@material-ui/core/Typography";
import { Link, Toast } from "@skbkontur/react-ui";
import { Roles } from "../../../../../../types";

interface Props {
  data: {
    teacher?: string;
    teacherContacts?: string;
    description?: string;
  };
  role?: string;
}

function copyEmailAddress(event?: React.MouseEvent<HTMLAnchorElement>) {
  navigator.clipboard.writeText(event!.currentTarget.textContent!).then(() => {
    Toast.push("Адрес скопирован");
  });
}

function needTeacher(props: Props) {
  return props.role === Roles.Student || props.role === Roles.Curator ? (
    <Typography variant="subtitle2">
      Преподаватель: {props.data.teacher},{" "}
      <Link use="default" onClick={copyEmailAddress}>
        {props.data.teacherContacts}
      </Link>
    </Typography>
  ) : null;
}

function Description(props: Props) {
  return (
    <div className="descriptionDetail">
      <Typography variant="h6">{props.data.description}</Typography>
      <hr />
      {needTeacher(props)}
    </div>
  );
}

export default Description;
